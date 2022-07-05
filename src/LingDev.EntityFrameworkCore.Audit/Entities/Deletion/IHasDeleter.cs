namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// A standard interface to add Deleter property.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface IHasDeleter<TUser> : ISoftDelete where TUser : class
{
    /// <summary>
    /// The user who deleted this entity.
    /// </summary>
    TUser? Deleter { get; set; }
}
