using LingDev.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class SeedServiceCollectionExtensions
{
    /// <summary>
    /// Add seed services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The builder.</returns>
    public static SeedDataBuilder AddSeedServices(this IServiceCollection services)
    {
        return new SeedDataBuilder(services);
    }

    /// <summary>
    /// Set the database to which the seed data should be applied。
    /// </summary>
    /// <typeparam name="TDbContext">The type of database context.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="setupOptions">The action used to configure the <see cref="SeedOptions"/>.</param>
    public static void UseDataBase<TDbContext>(this SeedDataBuilder builder, Action<SeedOptions>? setupOptions = null)
        where TDbContext : DbContext
    {
        builder.Services.ConfigureSeedOptions<TDbContext>(setupOptions);
        builder.Services.PostConfigure<SeedOptions>(typeof(TDbContext).Name, options =>
        {
            options.ModelTypes = builder.Types.ToArray();
        });

        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IDatabaseInitializer, DatabaseInitializer<TDbContext>>());
    }

    private static void ConfigureSeedOptions<T>(this IServiceCollection services, Action<SeedOptions>? setupAcion)
    {
        if (setupAcion == null)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<SeedOptions>, SeedConfigureOptions<T>>());
        }
        else
        {
            services.Configure(typeof(T).Name, setupAcion);
        }
    }
}
