using LingDev.EntityFrameworkCore.Audit.Models;

namespace LingDev.EntityFrameworkCore.Audit.Logger;

/// <summary>
/// Represents change information for an entity.
/// </summary>
public class EntityChangeInfo
{
    /// <summary>
    /// The primary key of the entity.
    /// </summary>
    public string EntityId { get; set; } = null!;

    /// <summary>
    /// The type of the entity.
    /// </summary>
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// The type of entity changed.
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// A collection of all changed property information for the entity.
    /// </summary>
    public IEnumerable<EntityPropertyChangeInfo> PropertyChanges { get; set; } = Enumerable.Empty<EntityPropertyChangeInfo>();
}
