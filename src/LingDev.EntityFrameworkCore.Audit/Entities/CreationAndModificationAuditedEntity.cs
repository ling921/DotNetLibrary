using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// Base class for entity has both created and modified user information.
/// </summary>
/// <typeparam name="TKey">The type of the primary key of this entity.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public abstract class CreationAndModificationAuditedEntity<TKey, TUser> : Entity<TKey>, ICreationAuditedEntity<TUser>, IModificationAuditedEntity<TUser>
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
}
