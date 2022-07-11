using LingDev.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseInitializer>();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var initializer = new DatabaseInitializer(logger, context);
        
    }
}
