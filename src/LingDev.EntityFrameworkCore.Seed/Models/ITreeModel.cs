namespace LingDev.EntityFrameworkCore.Seed.Models;

/// <summary>
/// Interface to the seed data model with children.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface ITreeModel<TEntity> : ISeedModel<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The children of the current seed data model.
    /// </summary>
    IEnumerable<ITreeModel<TEntity>> Children { get; set; }
}
