using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// This interface can be implemented to store deletion information.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public interface IDeletionAuditedEntity<TUser> : IEntity, IHasDeleter<TUser>, IHasDeletionTime, ISoftDelete
    where TUser : class
{
}
