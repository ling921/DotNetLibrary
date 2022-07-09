using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LingDev.EntityFrameworkCore.Audit;

/// <summary>
/// Configure for <see cref="AuditOptions"/>.
/// </summary>
public class AuditConfigureOptions : ConfigureFromConfigurationOptions<AuditOptions>
{
    /// <summary>
    /// Use the configuration section "Audit" to configure <see cref="AuditOptions"/>.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public AuditConfigureOptions(IConfiguration config) : base(config.GetSection("Audit"))
    {
    }
}
