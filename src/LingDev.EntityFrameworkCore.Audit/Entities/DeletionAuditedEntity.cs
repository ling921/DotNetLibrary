using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// Base class for entity has deleted user information.
/// </summary>
/// <typeparam name="TKey">The type of the primary key of this entity.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public abstract class DeletionAuditedEntity<TKey, TUser> : Entity<TKey>, IDeletionAuditedEntity<TUser>
    where TUser : class
{
    /// <inheritdoc/>
    public virtual bool IsDeleted { get; set; }

    /// <inheritdoc/>
    public virtual DateTimeOffset? DeletionTime { get; set; }

    /// <inheritdoc/>
    public virtual TUser? Deleter { get; set; }
}
