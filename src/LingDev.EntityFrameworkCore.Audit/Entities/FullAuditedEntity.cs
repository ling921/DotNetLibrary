using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// Base class for entity has created, modified and deleted user information.
/// </summary>
/// <typeparam name="TKey">The type of the primary key of this entity.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public abstract class FullAuditedEntity<TKey, TUser> : Entity<TKey>, ICreationAuditedEntity<TUser>, IModificationAuditedEntity<TUser>, IDeletionAuditedEntity<TUser>
    where TUser : class
{
    /// <inheritdoc/>
    public virtual DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc/>
    public virtual TUser? Creator { get; set; }

    /// <inheritdoc/>
    public virtual DateTimeOffset? LastModificationTime { get; set; }

    /// <inheritdoc/>
    public virtual TUser? LastModifier { get; set; }

    /// <inheritdoc/>
    public virtual bool IsDeleted { get; set; }

    /// <inheritdoc/>
    public virtual DateTimeOffset? DeletionTime { get; set; }

    /// <inheritdoc/>
    public virtual TUser? Deleter { get; set; }
}
