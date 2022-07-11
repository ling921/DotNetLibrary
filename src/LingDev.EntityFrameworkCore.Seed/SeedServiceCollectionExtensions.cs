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
    /// <param name="services"></param>
    /// <returns></returns>
    public static SeedDataBuilder AddSeedServices(this IServiceCollection services)
    {
        return new SeedDataBuilder(services);
    }

    public static void UseDataBase<TDbContext>(this SeedDataBuilder builder, Action<SeedOptions>? setupOptions = null)
        where TDbContext : DbContext
    {
        builder.Services.ConfigureSeedOptions(setupOptions);

        var types = builder.Types;
        builder.Services.AddTransient<DatabaseInitializer>();
    }

    private static void ConfigureSeedOptions(this IServiceCollection services, Action<SeedOptions>? setupAcion)
    {
        if (setupAcion == null)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<SeedOptions>, SeedConfigureOptions>());
        }
        else
        {
            var auditOptions = new SeedOptions();
            setupAcion.Invoke(auditOptions);
            services.AddSingleton<IOptions<SeedOptions>>(_ => Options.Options.Create(auditOptions));
        }
    }
}
