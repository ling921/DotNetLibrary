using LingDev.EntityFrameworkCore.Audit.Models;

namespace LingDev.EntityFrameworkCore.Audit.Internal.Models;

internal class EntityChangeInfo
{
    public string EntityId { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public EventType EventType { get; set; }

    public List<EntityPropertyChangeInfo> PropertyChanges { get; set; } = new();
}
