using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingDev.EntityFrameworkCore.Entities;

/// <summary>
/// Defines an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">Type of the primary key of the entity.</typeparam>
public abstract class Entity<TKey> : IEntity<TKey>
{
    /// <inheritdoc/>
    [Key]
    [Column(Order = 0)]
    [Comment("Unique identifier for this entity.")]
    public virtual TKey Id { get; set; } = default!;

    /// <inheritdoc/>
    public virtual object?[] GetKeys() => new object?[] { Id };
}
