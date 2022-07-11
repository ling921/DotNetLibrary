using LingDev.EntityFrameworkCore.Seed.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LingDev.EntityFrameworkCore.Seed
{
    /// <summary>
    /// Used to configure seed services.
    /// </summary>
    public class SeedDataBuilder
    {
        internal readonly HashSet<Type> Types = new();

        /// <summary>
        /// The services being configured.
        /// </summary>
        public virtual IServiceCollection Services { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SeedDataBuilder"/>.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        public SeedDataBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Adds a seed model.
        /// </summary>
        /// <typeparam name="TModel">The type of seed model.</typeparam>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>The builder.</returns>
        public virtual SeedDataBuilder AddModel<TModel, TEntity>()
            where TModel : class, ISeedModel<TEntity>
            where TEntity : class
        {
            var type = typeof(TModel);
            if (!Types.Contains(type))
            {
                Types.Add(type);
                Services.AddTransient<TModel>();
            }
            return this;
        }

        /// <summary>
        /// Add seed models from the assembly that inherits from the interface <see cref="ISeedModel{TEntity}"/>
        /// </summary>
        /// <param name="assembly">The assembly with seed models.</param>
        /// <returns>The builder.</returns>
        public virtual SeedDataBuilder AddAssembly(Assembly assembly)
        {
            var interfaceType = typeof(ISeedModel<>);
            var types = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && HasInterface(type, interfaceType));
            foreach (var type in types)
            {
                if (!Types.Contains(type))
                {
                    Types.Add(type);
                    Services.AddTransient(type);
                }
            }
            return this;
        }

        private static bool HasInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }
    }
}
