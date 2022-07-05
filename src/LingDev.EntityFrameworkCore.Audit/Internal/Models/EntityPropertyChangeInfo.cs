namespace LingDev.EntityFrameworkCore.Audit.Internal.Models;

internal class EntityPropertyChangeInfo
{
    public string PropertyName { get; set; } = null!;

    public string? OriginalValue { get; set; }

    public string? NewValue { get; set; }
}
