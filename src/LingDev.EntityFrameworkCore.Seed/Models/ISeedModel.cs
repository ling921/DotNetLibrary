namespace LingDev.EntityFrameworkCore.Seed.Models;

/// <summary>
/// Interface to the seed data model.
/// </summary>
/// <typeparam name="TModel">The type of seed data model.</typeparam>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface ISeedModel<TModel, TEntity>
    where TModel : class, ISeedModel<TModel, TEntity>
    where TEntity : class
{
    /// <summary>
    /// Returns an entity instance that represents the current seed data model.
    /// </summary>
    /// <returns>An entity instance that represents the current seed data model.</returns>
    TEntity ToEntity();
}
