using System.ComponentModel.DataAnnotations;

namespace LingDev.EntityFrameworkCore.Entities;

/// <summary>
/// Defines an entity. It's primary key may not be "Id" or it may have a composite primary key.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object?[] GetKeys();
}

/// <summary>
/// Defines an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">Type of the primary key of the entity.</typeparam>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    [Key]
    TKey Id { get; set; }
}
