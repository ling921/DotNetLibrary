namespace LingDev.EntityFrameworkCore.Seed.Models;

/// <summary>
/// Interface to the seed data model with children.
/// </summary>
/// <typeparam name="TModel">The type of seed model.</typeparam>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface ITreeSeedModel<TModel, TEntity> : ISeedModel<TEntity>
    where TModel : class, ISeedModel<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The children of the current seed data model.
    /// </summary>
    IEnumerable<TModel> Children { get; set; }
}
