using LingDev.EntityFrameworkCore.Audit.Entities;
using LingDev.EntityFrameworkCore.Audit.Internal;
using LingDev.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LingDev.EntityFrameworkCore.Audit.Extensions;

/// <summary>
/// Extension methods for building model.
/// </summary>
public static class ModelBuilderExtensions
{
    #region Public Methods

    /// <summary>
    /// Configure properties of audit entities.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <param name="modelBuilder">Entity model builder.</param>
    /// <param name="comments">Comments to the audited entities.</param>
    public static void ConfigureAuditEntityProperties<TUser>(this ModelBuilder modelBuilder, AuditEntityComments comments)
        where TUser : class, IEntity
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var type = entityType.ClrType;

            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)))
            {
                GetMethod(nameof(ConfigurePrimaryKey), type)
                    .Invoke(null, new object[] { modelBuilder, comments });
            }

            if (typeof(IHasCreationTime).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasCreationTimeEntity), type)
                    .Invoke(null, new object[] { modelBuilder, comments });
            }
            if (typeof(IHasCreator<TUser>).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasCreatorEntity), type, typeof(TUser))
                    .Invoke(null, new object[] { modelBuilder, comments });
            }

            if (typeof(IHasModificationTime).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasModificationTimeEntity), type)
                    .Invoke(null, new object[] { modelBuilder, comments });
            }
            if (typeof(IHasModifier<TUser>).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasModifierEntity), type, typeof(TUser))
                    .Invoke(null, new object[] { modelBuilder, comments });
            }

            if (typeof(IHasDeletionTime).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasDeletionTimeEntity), type)
                    .Invoke(null, new object[] { modelBuilder, comments });
            }
            if (typeof(IHasDeleter<TUser>).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureHasDeleterEntity), type, typeof(TUser))
                    .Invoke(null, new object[] { modelBuilder, comments });
            }

            if (typeof(ISoftDelete).IsAssignableFrom(type))
            {
                GetMethod(nameof(ConfigureSoftDeleteFilter), type)
                    .Invoke(null, new object[] { modelBuilder, comments });
            }
        }
    }

    #endregion Public Methods

    #region Private Methods

    private static MethodInfo GetMethod(string name, params Type[] typeArguments)
    {
        return typeof(ModelBuilderExtensions)
            .GetMethod(
                name,
                BindingFlags.NonPublic | BindingFlags.Static
            )!
            .MakeGenericMethod(typeArguments);
    }

    private static void ConfigurePrimaryKey<TEntity>(ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IEntity
    {
        builder.Entity<TEntity>()
            .Property(AuditConstants.Id)
            .HasComment(comments.Id);
    }

    private static void ConfigureHasCreationTimeEntity<TEntity>(ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasCreationTime
    {
        builder.Entity<TEntity>()
            .Property(e => e.CreationTime)
            .HasComment(comments.CreationTime);
    }

    private static void ConfigureHasCreatorEntity<TEntity, TUser>(ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasCreator<TUser>
        where TUser : class, IEntity
    {
        builder.Entity<TEntity>()
            .HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(AuditConstants.CreatorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TEntity>()
            .Property(AuditConstants.CreatorId)
            .HasComment(comments.CreatorId);
    }

    private static void ConfigureHasModificationTimeEntity<TEntity>(ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasModificationTime
    {
        builder.Entity<TEntity>()
            .Property(e => e.LastModificationTime)
            .HasComment(comments.LastModificationTime);
    }

    private static void ConfigureHasModifierEntity<TEntity, TUser>(ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasModifier<TUser>
        where TUser : class, IEntity
    {
        builder.Entity<TEntity>()
            .HasOne(e => e.LastModifier)
            .WithMany()
            .HasForeignKey(AuditConstants.LastModifierId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TEntity>()
            .Property(AuditConstants.LastModifierId)
            .HasComment(comments.LastModifierId);
    }

    private static void ConfigureHasDeletionTimeEntity<TEntity>(this ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasDeletionTime
    {
        builder.Entity<TEntity>()
            .Property(e => e.DeletionTime)
            .HasComment(comments.DeletionTime);
    }

    private static void ConfigureHasDeleterEntity<TEntity, TUser>(this ModelBuilder builder, AuditEntityComments comments)
        where TEntity : class, IHasDeleter<TUser>
        where TUser : class, IEntity
    {
        builder.Entity<TEntity>()
            .HasOne(e => e.Deleter)
            .WithMany()
            .HasForeignKey(AuditConstants.DeleterId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TEntity>()
            .Property(AuditConstants.DeleterId)
            .HasComment(comments.DeleterId);
    }

    private static void ConfigureSoftDeleteFilter<TEntity>(this ModelBuilder modelBuilder, AuditEntityComments comments)
        where TEntity : class, ISoftDelete
    {
        modelBuilder.Entity<TEntity>()
            .Property(e => e.IsDeleted)
            .HasComment(comments.IsDeleted);

        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => !entity.IsDeleted);
    }

    #endregion Private Methods
}
