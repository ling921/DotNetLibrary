namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// Interface to make your entity "soft delete".
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Used to mark an entity as deleted instead of actually deleting it.
    /// </summary>
    bool IsDeleted { get; set; }
}
