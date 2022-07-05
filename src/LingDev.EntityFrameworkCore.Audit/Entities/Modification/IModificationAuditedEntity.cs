using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// This interface can be implemented to store modification information.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface IModificationAuditedEntity<TUser> : IEntity, IHasModifier<TUser>, IHasModificationTime
    where TUser : class
{
}
