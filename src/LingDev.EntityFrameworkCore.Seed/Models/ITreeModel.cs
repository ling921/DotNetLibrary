namespace LingDev.EntityFrameworkCore.Seed.Models;

/// <summary>
/// Interface to the seed data model with children.
/// </summary>
/// <typeparam name="TModel">The type of seed data model.</typeparam>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface ITreeModel<TModel, TEntity> : ISeedModel<TModel, TEntity>
    where TModel : class, ITreeModel<TModel, TEntity>
    where TEntity : class
{
    /// <summary>
    /// The children of the current seed data model.
    /// </summary>
    IEnumerable<ITreeModel<TModel, TEntity>> Children { get; set; }
}
