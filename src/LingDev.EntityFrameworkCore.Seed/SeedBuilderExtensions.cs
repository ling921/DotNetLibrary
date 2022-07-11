using LingDev.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class SeedBuilderExtensions
{
    /// <summary>
    /// Apply seed data when app started.
    /// </summary>
    /// <param name="app"></param>
    public static void UseSeedData<TDbContext>(this IApplicationBuilder app)
        where TDbContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializers = scope.ServiceProvider.GetServices<IDatabaseInitializer>();
        foreach (var initializer in initializers)
        {
            initializer.RunAsync().Wait();
        }
    }
}
