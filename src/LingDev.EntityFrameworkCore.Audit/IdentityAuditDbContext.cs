using LingDev.EntityFrameworkCore.Audit.Extensions;
using LingDev.EntityFrameworkCore.Audit.Identity;
using LingDev.EntityFrameworkCore.Internal.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LingDev.EntityFrameworkCore.Audit;

/// <summary>
/// Base class for the Entity Framework database context used for identity and auditing.
/// </summary>
/// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
/// <inheritdoc/>
public abstract class IdentityAuditDbContext<TDbContext>
    : IdentityAuditDbContext<
        TDbContext,
        IdentityUser<Guid>,
        IdentityRole<Guid>,
        Guid>,
    IAuditDbContext<IdentityUser<Guid>>
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected IdentityAuditDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity and auditing..
/// </summary>
/// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
/// <inheritdoc/>
public abstract class IdentityAuditDbContext<TDbContext, TUser, TKey>
    : IdentityAuditDbContext<
        TDbContext,
        TUser,
        IdentityRole<TKey>,
        TKey,
        IdentityUserClaim<TKey>,
        IdentityUserRole<TKey>,
        IdentityUserLogin<TKey>,
        IdentityRoleClaim<TKey>,
        IdentityUserToken<TKey>>,
    IAuditDbContext<TUser>
    where TDbContext : DbContext
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected IdentityAuditDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity and auditing..
/// </summary>
/// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
/// <inheritdoc/>
public abstract class IdentityAuditDbContext<TDbContext, TUser, TRole, TKey>
    : IdentityAuditDbContext<
        TDbContext,
        TUser,
        TRole,
        TKey,
        IdentityUserClaim<TKey>,
        IdentityUserRole<TKey>,
        IdentityUserLogin<TKey>,
        IdentityRoleClaim<TKey>,
        IdentityUserToken<TKey>>,
    IAuditDbContext<TUser>
    where TDbContext : DbContext
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected IdentityAuditDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity and auditing.
/// </summary>
/// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
/// <typeparam name="TUserClaim">The type of user claim objects.</typeparam>
/// <typeparam name="TUserRole">The type of user role objects.</typeparam>
/// <typeparam name="TUserLogin">The type of user login objects.</typeparam>
/// <typeparam name="TRoleClaim">The type of role claim objects.</typeparam>
/// <typeparam name="TUserToken">The type of user token objects.</typeparam>
/// <inheritdoc/>
public abstract class IdentityAuditDbContext<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    : IdentityDbContext<
        TUser,
        TRole,
        TKey,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken>,
    IAuditDbContext<TUser>
    where TDbContext : DbContext
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
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
    protected IdentityAuditDbContext(DbContextOptions<TDbContext> options) : base(options)
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
    protected override sealed void OnModelCreating(ModelBuilder builder)
    {
        var options = this.GetService<IOptionsSnapshot<AuditOptions>>().Value;

        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new AuditLogTypeConfiguration<TUser>());
        builder.ApplyConfiguration(new AuditLogDetailTypeConfiguration());

        builder.ApplyConfiguration(new IdentityUserTypeConfiguration<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>());
        builder.ApplyConfiguration(new IdentityRoleTypeConfiguration<TRole, TRoleClaim, TKey>());
        builder.ApplyConfiguration(new IdentityUserClaimTypeConfiguration<TUserClaim, TKey>());
        builder.ApplyConfiguration(new IdentityUserRoleTypeConfiguration<TUserRole, TKey>());
        builder.ApplyConfiguration(new IdentityUserLoginTypeConfiguration<TUserLogin, TKey>());
        builder.ApplyConfiguration(new IdentityRoleClaimTypeConfiguration<TRoleClaim, TKey>());
        builder.ApplyConfiguration(new IdentityUserTokenTypeConfiguration<TUserToken, TKey>());

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
