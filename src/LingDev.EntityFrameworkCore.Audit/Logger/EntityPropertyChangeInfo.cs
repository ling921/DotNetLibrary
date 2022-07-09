namespace LingDev.EntityFrameworkCore.Audit.Logger;

/// <summary>
/// Represents the property change information of the entity.
/// </summary>
public class EntityPropertyChangeInfo
{
    /// <summary>
    /// The property name.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// The original value of the property.
    /// </summary>
    public object? OriginalValue { get; set; }

    /// <summary>
    /// The new value of the property.
    /// </summary>
    public object? NewValue { get; set; }
}
