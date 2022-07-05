namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// A standard interface to add LastModifier property.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface IHasModifier<TUser> where TUser : class
{
    /// <summary>
    /// The user who modified this entity.
    /// </summary>
    TUser? LastModifier { get; set; }
}
