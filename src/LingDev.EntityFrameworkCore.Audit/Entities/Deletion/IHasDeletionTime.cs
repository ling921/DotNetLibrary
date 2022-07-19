namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// A standard interface to add DeletionTime property to a class. It also makes the class soft delete.
/// </summary>
public interface IHasDeletionTime : ISoftDelete
{
    /// <summary>
    /// The deleted time for this entity.
    /// </summary>
    DateTimeOffset? DeletionTime { get; set; }
}
