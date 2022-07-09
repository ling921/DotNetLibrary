using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Audit.Entities;
using LingDev.EntityFrameworkCore.Audit.Internal;
using LingDev.EntityFrameworkCore.Audit.Logger;
using LingDev.EntityFrameworkCore.Audit.Models;
using LingDev.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace LingDev.EntityFrameworkCore.Audit.Extensions;

/// <summary>
/// A helper class to get entity changes and save them to the database.
/// </summary>
public static class AuditHelper
{
    private static readonly ConcurrentDictionary<string, string[]> _auditPropertyDictionary = new();

    /// <summary>
    /// Saves all changes and audit logs to the database.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <param name="context">The instance of audit databse context.</param>
    /// <param name="baseSaveChanges">A delegate to save all changes to the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public static async Task<int> SaveChangesAsync<TUser>(
        IAuditDbContext<TUser> context,
        Func<Task<int>> baseSaveChanges,
        CancellationToken cancellationToken)
        where TUser : class
    {
        var ctx = context.DbContext;
        var user = context.Operator;
        var options = context.Options;

        var transaction = ctx.Database.CurrentTransaction;
        var createFlag = false;
        var auditSavePointName = Guid.NewGuid().ToString();
        if (transaction == null)
        {
            transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
            createFlag = true;
        }
        else
        {
            await transaction.CreateSavepointAsync(auditSavePointName, cancellationToken);
        }
        try
        {
            var list = AuditEntries(context).ToList();
            var auditLogs = list
                .Where(i => i.EventType != EventType.Create)
                .Select(i => GetAuditLog(i.Entry, i.EventType, user))
                .ToList();

            var result = await baseSaveChanges();

            auditLogs.AddRange(list.Where(i => i.EventType == EventType.Create).Select(i => GetAuditLog(i.Entry, i.EventType, user)));
            await ctx.AddRangeAsync(auditLogs, cancellationToken);

            await baseSaveChanges();
            context.Logger.LogDebug("Logged changes of entities.");

            if (createFlag)
            {
                await transaction.CommitAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "An exception occurred while logging audit changes to the database.");
            if (createFlag)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }
            else
            {
                await transaction.RollbackToSavepointAsync(auditSavePointName, cancellationToken);
            }
            throw;
        }
    }

    /// <summary>
    /// Saves all changes and audit logs to the database.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <param name="context">The instance of audit databse context.</param>
    /// <param name="baseSaveChanges">A delegate to save all changes to the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public static int SaveChanges<TUser>(
        IAuditDbContext<TUser> context,
        Func<int> baseSaveChanges)
        where TUser : class
    {
        var ctx = context.DbContext;
        var user = context.Operator;
        var options = context.Options;

        var transaction = ctx.Database.CurrentTransaction;
        var createFlag = false;
        var auditSavePointName = Guid.NewGuid().ToString();
        if (transaction == null)
        {
            transaction = ctx.Database.BeginTransaction();
            createFlag = true;
        }
        else
        {
            transaction.CreateSavepoint(auditSavePointName);
        }
        try
        {
            var list = AuditEntries(context).ToList();
            var auditLogs = list
                .Where(i => i.EventType != EventType.Create)
                .Select(i => GetAuditLog(i.Entry, i.EventType, user))
                .ToList();

            var result = baseSaveChanges();

            auditLogs.AddRange(list.Where(i => i.EventType == EventType.Create).Select(i => GetAuditLog(i.Entry, i.EventType, user)));
            ctx.AddRange(auditLogs);

            baseSaveChanges();
            context.Logger.LogDebug("Logged changes of entities.");

            if (createFlag)
            {
                transaction.Commit();
                transaction.Dispose();
            }

            return result;
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "An exception occurred while logging audit changes to the database.");
            if (createFlag)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
            else
            {
                transaction.RollbackToSavepoint(auditSavePointName);
            }
            throw;
        }
    }

    internal static IEnumerable<(EntityEntry Entry, EventType EventType)> AuditEntries<TUser>(IAuditDbContext<TUser> context)
        where TUser : class
    {
        var now = DateTimeOffset.Now;
        var user = context.Operator;
        var options = context.Options;
        foreach (var entityEntry in context.DbContext.ChangeTracker.Entries())
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
                        if (entityType != typeof(TUser)
                            && !options.AllowAnonymousCreate
                            && !allowedOperation.HasFlag(DbOperation.Create)
                            && user == null)
                        {
                            throw new InvalidOperationException($"Anonymous creation of {entityType.Name} is not allowed.");
                        }
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

    internal static AuditLog<TUser> GetAuditLog<TUser>(EntityEntry entityEntry, EventType eventType, TUser? user)
        where TUser : class
    {
        var entityChangeInfo = GetEntityChangeInfo(entityEntry, eventType);

        return new AuditLog<TUser>
        {
            EntityId = entityChangeInfo.EntityId,
            EntityType = entityChangeInfo.EntityType,
            EventType = entityChangeInfo.EventType,
            EventTime = DateTimeOffset.Now,
            Operator = user,
            Details = entityChangeInfo.PropertyChanges.Select(i => new AuditLogDetail
            {
                PropertyName = entityChangeInfo.EntityType + '.' + i.PropertyName,
                OriginalValue = GetStringValue(i.OriginalValue),
                NewValue = GetStringValue(i.NewValue),
            }).ToList()
        };
    }

    #region Private Methods

    private static EntityChangeInfo GetEntityChangeInfo(EntityEntry entityEntry, EventType eventType)
    {
        var entityId = GetEntityId(entityEntry.Entity) ?? string.Empty;
        var entityType = entityEntry.Entity.GetType().GetFriendlyTypeName();
        var propertyChanges = GetPropertyChanges(entityEntry, eventType);

        return new EntityChangeInfo
        {
            EntityId = entityId,
            EntityType = entityType,
            EventType = eventType,
            PropertyChanges = propertyChanges
        };
    }

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

    private static IEnumerable<EntityPropertyChangeInfo> GetPropertyChanges(EntityEntry entityEntry, EventType eventType)
    {
        if (eventType is EventType.Create or EventType.Delete)
        {
            yield break;
        }

        var entityType = entityEntry.Entity.GetType();
        if (entityType.IsDefined(typeof(DisableAuditingAttribute), true))
        {
            yield break;
        }

        if (_auditPropertyDictionary.TryGetValue(entityType.Name, out var auditProperties))
        {
            foreach (var propertyName in auditProperties)
            {
                var propertyEntry = entityEntry.Property(propertyName);
                if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                    continue;
                yield return new EntityPropertyChangeInfo
                {
                    PropertyName = propertyName,
                    OriginalValue = propertyEntry.OriginalValue,
                    NewValue = propertyEntry.CurrentValue
                };
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
                    yield return new EntityPropertyChangeInfo
                    {
                        PropertyName = property.Name,
                        OriginalValue = propertyEntry.OriginalValue,
                        NewValue = propertyEntry.CurrentValue
                    };
                }
            }
            _auditPropertyDictionary.TryAdd(entityType.Name, propertyNames.ToArray());
        }
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

    private static string GetFriendlyTypeName(this Type type)
    {
        string friendlyName = type.Name;
        if (type.IsGenericType)
        {
            int iBacktick = friendlyName.IndexOf('`');
            if (iBacktick > 0)
            {
                friendlyName = friendlyName.Remove(iBacktick);
            }
            friendlyName += "<";
            Type[] typeParameters = type.GetGenericArguments();
            for (int i = 0; i < typeParameters.Length; ++i)
            {
                string typeParamName = typeParameters[i].GetFriendlyTypeName();
                friendlyName += i == 0 ? typeParamName : "," + typeParamName;
            }
            friendlyName += ">";
        }

        return friendlyName;
    }

    #endregion Private Methods
}
