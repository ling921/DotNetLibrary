namespace LingDev.EntityFrameworkCore.Audit.Entities;

/// <summary>
/// A standard interface to add CreationTime property.
/// </summary>
public interface IHasCreationTime
{
    /// <summary>
    /// The created time for this entity.
    /// </summary>
    DateTimeOffset CreationTime { get; set; }
}
