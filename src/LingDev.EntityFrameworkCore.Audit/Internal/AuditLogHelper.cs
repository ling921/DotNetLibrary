using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Audit.Entities;
using LingDev.EntityFrameworkCore.Audit.Internal.Models;
using LingDev.EntityFrameworkCore.Audit.Models;
using LingDev.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace LingDev.EntityFrameworkCore.Audit.Internal
{
    internal static class AuditLogHelper
    {
        private static readonly ConcurrentDictionary<string, string[]> _auditPropertyDictionary = new();

        public static IEnumerable<(EntityEntry Entry, EventType EventType)> AuditEntries<TUser>(
            IEnumerable<EntityEntry> entityEntries,
            TUser? user,
            AuditOptions options)
            where TUser : class
        {
            var now = DateTimeOffset.Now;
            foreach (var entityEntry in entityEntries)
            {
                var entityType = entityEntry.Metadata.ClrType;
                var allowedOperation = GetEntityAllowedAnonymousOperation(entityType);
                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                        if (entityEntry.Entity is IHasDeletionTime entity1)
                        {
                            entity1.DeletionTime = now;
                        }
                        if (entityEntry.Entity is IHasDeleter<TUser> entity2)
                        {
                            if (!options.AllowAnonymousDelete && !allowedOperation.HasFlag(DbOperation.Delete) && user == null)
                                throw new InvalidOperationException($"Anonymous deletion of {entityType.Name} is not allowed.");
                            entity2.Deleter = user;
                        }
                        if (entityEntry.Entity is ISoftDelete entity3)
                        {
                            entity3.IsDeleted = true;
                            entityEntry.State = EntityState.Modified;
                            yield return (entityEntry, EventType.SoftDelete);
                        }
                        else
                        {
                            yield return (entityEntry, EventType.Delete);
                        }
                        break;

                    case EntityState.Modified:
                        if (entityEntry.Entity is IHasModificationTime entity4)
                        {
                            entity4.LastModificationTime = now;
                        }
                        if (entityEntry.Entity is IHasModifier<TUser> entity5)
                        {
                            if (!options.AllowAnonymousModify && !allowedOperation.HasFlag(DbOperation.Update) && user == null)
                                throw new InvalidOperationException($"Anonymous modification of {entityType.Name} is not allowed.");
                            entity5.LastModifier = user;
                        }
                        yield return (entityEntry, GetModifiedType(entityEntry));
                        break;

                    case EntityState.Added:
                        if (entityEntry.Entity is IHasCreationTime entity6)
                        {
                            entity6.CreationTime = now;
                        }
                        if (entityEntry.Entity is IHasCreator<TUser> entity7)
                        {
                            if (!options.AllowAnonymousCreate && !allowedOperation.HasFlag(DbOperation.Create) && user == null)
                                throw new InvalidOperationException($"Anonymous creation of {entityType.Name} is not allowed.");
                            entity7.Creator = user;
                        }
                        yield return (entityEntry, EventType.Create);
                        break;

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }
            }
        }

        public static AuditLog<TUser> GetAuditLog<TUser>(EntityEntry entityEntry, EventType eventType, TUser? user)
            where TUser : class
        {
            var entityId = GetEntityId(entityEntry.Entity) ?? string.Empty;
            var entityType = entityEntry.Entity.GetType().Name;
            var propertyChanges = GetPropertyChanges(entityEntry, eventType);

            return new AuditLog<TUser>
            {
                EntityId = entityId,
                EntityType = entityType,
                EventType = eventType,
                EventTime = DateTimeOffset.Now,
                Operator = user,
                Details = propertyChanges.ConvertAll(pc => new AuditLogDetail
                {
                    PropertyName = pc.PropertyName,
                    OriginalValue = pc.OriginalValue,
                    NewValue = pc.NewValue,
                })
            };
        }

        #region Private Methods

        private static EventType GetModifiedType(EntityEntry entityEntry)
        {
            if (entityEntry.Entity is ISoftDelete)
            {
                var originalValue = (bool)entityEntry.Property(nameof(ISoftDelete.IsDeleted)).OriginalValue!;
                var newValue = (bool)entityEntry.Property(nameof(ISoftDelete.IsDeleted)).CurrentValue!;
                if (originalValue != newValue)
                {
                    return newValue ? EventType.SoftDelete : EventType.Recovery;
                }
            }
            return EventType.Modify;
        }

        private static string? GetEntityId(object entityAsObj)
        {
            if (entityAsObj is IEntity entity)
            {
                var keys = entity.GetKeys();
                if (keys == null || keys.All(k => k == null))
                {
                    return null;
                }
                return string.Join(",", keys);
            }
            return null;
        }

        private static List<EntityPropertyChangeInfo> GetPropertyChanges(EntityEntry entityEntry, EventType eventType)
        {
            var propertyChanges = new List<EntityPropertyChangeInfo>();

            if (eventType is EventType.Create or EventType.Delete)
            {
                return propertyChanges;
            }

            var entityType = entityEntry.Entity.GetType();
            if (entityType.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return propertyChanges;
            }

            if (_auditPropertyDictionary.TryGetValue(entityType.Name, out var auditProperties))
            {
                foreach (var propertyName in auditProperties)
                {
                    var propertyEntry = entityEntry.Property(propertyName);
                    if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                        continue;
                    propertyChanges.Add(new EntityPropertyChangeInfo
                    {
                        PropertyName = entityType.Name + "." + propertyName,
                        OriginalValue = GetStringValue(propertyEntry.OriginalValue),
                        NewValue = GetStringValue(propertyEntry.CurrentValue),
                    });
                }
            }
            else
            {
                var propertyNames = new List<string>();
                foreach (var property in entityEntry.Metadata.GetProperties())
                {
                    var propertyEntry = entityEntry.Property(property.Name);
                    if (ShouldSavePropertyHistory(propertyEntry))
                    {
                        propertyNames.Add(property.Name);

                        if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                            continue;
                        propertyChanges.Add(new EntityPropertyChangeInfo
                        {
                            PropertyName = entityType.Name + "." + property.Name,
                            OriginalValue = GetStringValue(propertyEntry.OriginalValue),
                            NewValue = GetStringValue(propertyEntry.CurrentValue),
                        });
                    }
                }
                _auditPropertyDictionary.TryAdd(entityType.Name, propertyNames.ToArray());
            }

            return propertyChanges;
        }

        private static bool ShouldSavePropertyHistory(PropertyEntry propertyEntry)
        {
            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo == null)
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (AuditConstants.PropertieNames.Contains(propertyInfo.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static string? GetStringValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                type = underlyingType;
            }

            var result = type.IsClass && type != typeof(string)
                ? JsonSerializer.Serialize(value)
                : value.ToString();

            return result == null || result.Length <= 128
                ? result
                : $"{result[..125]}...";
        }

        private static DbOperation GetEntityAllowedAnonymousOperation(Type entityType)
        {
            return entityType.GetCustomAttribute<AnonymousAuditingAttribute>()
                ?.Operation
                ?? DbOperation.None;
        }

        #endregion Private Methods
    }
}
