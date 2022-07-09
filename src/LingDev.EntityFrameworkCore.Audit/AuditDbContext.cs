using LingDev.EntityFrameworkCore.Audit.Extensions;
using LingDev.EntityFrameworkCore.Entities;
using LingDev.EntityFrameworkCore.Internal.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LingDev.EntityFrameworkCore.Audit;

/// <summary>
/// <see cref="DbContext"/> for auditing.
/// </summary>
/// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public abstract class AuditDbContext<TDbContext, TUser> : DbContext, IAuditDbContext<TUser>
    where TDbContext : DbContext
    where TUser : class, IEntity
{
    /// <inheritdoc/>
    public ILogger Logger { get; }

    /// <inheritdoc/>
    public DbContext DbContext => this;

    /// <inheritdoc/>
    public abstract TUser? Operator { get; }

    /// <inheritdoc/>
    public AuditOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected AuditDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        Logger = this.GetService<ILoggerFactory>().CreateLogger<TDbContext>();
        Options = this.GetService<IOptionsSnapshot<AuditOptions>>().Value;
    }

    /// <inheritdoc/>
    public override sealed async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var saveChanges = () => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        return await AuditHelper.SaveChangesAsync(this, saveChanges, cancellationToken);
    }

    /// <inheritdoc/>
    public override sealed int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return AuditHelper.SaveChanges(this, () => base.SaveChanges(acceptAllChangesOnSuccess));
    }

    /// <inheritdoc/>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected sealed override void OnModelCreating(ModelBuilder builder)
    {
        var options = this.GetService<IOptionsSnapshot<AuditOptions>>().Value;

        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new AuditLogTypeConfiguration<TUser>());
        builder.ApplyConfiguration(new AuditLogDetailTypeConfiguration());

        ConfigureModels(builder);

        // Audit properties for entities must be configured at the end.
        builder.ConfigureAuditEntityProperties<TUser>(options.Comments);
    }

    /// <summary>
    /// Configure the entity models.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected virtual void ConfigureModels(ModelBuilder builder)
    {
    }
}
