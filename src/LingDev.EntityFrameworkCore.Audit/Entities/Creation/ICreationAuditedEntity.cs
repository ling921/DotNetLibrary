using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// This interface can be implemented to store creation information.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface ICreationAuditedEntity<TUser> : IEntity, IHasCreator<TUser>, IHasCreationTime
    where TUser : class
{
}
