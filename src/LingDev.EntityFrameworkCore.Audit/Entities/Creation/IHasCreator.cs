namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// A standard interface to add Creator property.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface IHasCreator<TUser> where TUser : class
{
    /// <summary>
    /// The user who created this entity.
    /// </summary>
    TUser? Creator { get; set; }
}
