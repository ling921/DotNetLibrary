using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// Base class for entity has modified user information.
/// </summary>
/// <typeparam name="TKey">The type of the primary key of this entity.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public abstract class ModificationAuditedEntity<TKey, TUser> : Entity<TKey>, IModificationAuditedEntity<TUser>
    where TUser : class
{
    /// <inheritdoc/>
    public virtual DateTimeOffset? LastModificationTime { get; set; }

    /// <inheritdoc/>
    public virtual TUser? LastModifier { get; set; }
}
