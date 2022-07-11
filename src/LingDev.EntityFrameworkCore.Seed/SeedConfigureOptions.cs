using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LingDev.EntityFrameworkCore.Seed;

/// <summary>
/// Configure for <see cref="SeedOptions"/>.
/// </summary>
public class SeedConfigureOptions : ConfigureFromConfigurationOptions<SeedOptions>
{
    /// <summary>
    /// Use the configuration section "Seed" to configure <see cref="SeedOptions"/>.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public SeedConfigureOptions(IConfiguration config) : base(config.GetSection("Seed"))
    {
    }
}
