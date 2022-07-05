using LingDev.EntityFrameworkCore.Audit.Identity;
using LingDev.EntityFrameworkCore.Audit.Internal;
using LingDev.EntityFrameworkCore.Audit.Models;
using LingDev.EntityFrameworkCore.Internal.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

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
        Guid>
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
        IdentityUserToken<TKey>>
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
        IdentityUserToken<TKey>>
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
        TUserToken>
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
    private const string _auditSavePointName = "LingDev.EntityFrameworkCore.Audit";

    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected IdentityAuditDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        Logger = this.GetService<ILoggerFactory>().CreateLogger<TDbContext>();
    }

    /// <inheritdoc/>
    public override sealed async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var options = GetAuditOptions();
        var userId = GetOperatorId();
        var user = userId == null
            ? null
            : await Set<TUser>().FindAsync(new object?[] { userId }, cancellationToken);

        var transaction = Database.CurrentTransaction;
        var createFlag = false;
        if (transaction == null)
        {
            transaction = await Database.BeginTransactionAsync(cancellationToken);
            createFlag = true;
        }
        else
        {
            await transaction.CreateSavepointAsync(_auditSavePointName, cancellationToken);
        }

        try
        {
            var list = AuditLogHelper.AuditEntries(ChangeTracker.Entries(), user, options).ToList();
            var auditLogs = list
                .Where(i => i.EventType != EventType.Create)
                .Select(i => AuditLogHelper.GetAuditLog(i.Entry, i.EventType, user))
                .ToList();

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            auditLogs.AddRange(list.Where(i => i.EventType == EventType.Create).Select(i => AuditLogHelper.GetAuditLog(i.Entry, i.EventType, user)));
            await AddRangeAsync(auditLogs, cancellationToken);

            await base.SaveChangesAsync(true, cancellationToken);
            Logger.LogDebug("Logged changes of entities.");

            if (createFlag)
            {
                await transaction.CommitAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception occurred while logging audit changes to the database.");
            if (createFlag)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }
            else
            {
                await transaction.RollbackToSavepointAsync(_auditSavePointName, cancellationToken);
            }
            throw;
        }
    }

    /// <inheritdoc/>
    public override sealed int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var options = GetAuditOptions();
        var userId = GetOperatorId();
        var user = userId == null
            ? null
            : Set<TUser>().Find(userId);

        var transaction = Database.CurrentTransaction;
        var createFlag = false;
        if (transaction == null)
        {
            transaction = Database.BeginTransaction();
            createFlag = true;
        }
        else
        {
            transaction.CreateSavepoint(_auditSavePointName);
        }

        try
        {
            var list = AuditLogHelper.AuditEntries(ChangeTracker.Entries(), user, options).ToList();
            var auditLogs = list
                .Where(i => i.EventType != EventType.Create)
                .Select(i => AuditLogHelper.GetAuditLog(i.Entry, i.EventType, user))
                .ToList();

            var result = base.SaveChanges(acceptAllChangesOnSuccess);

            auditLogs.AddRange(list.Where(i => i.EventType == EventType.Create).Select(i => AuditLogHelper.GetAuditLog(i.Entry, i.EventType, user)));
            AddRange(auditLogs);

            base.SaveChanges(true);
            Logger.LogDebug("Logged changes of entities.");

            if (createFlag)
            {
                transaction.Commit();
                transaction.Dispose();
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception occurred while logging audit changes to the database.");
            if (createFlag)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
            else
            {
                transaction.RollbackToSavepoint(_auditSavePointName);
            }
            throw;
        }
    }

    /// <summary>
    /// Get the Id of the current operator.
    /// </summary>
    /// <returns>The Id of the current operator if exists, otherwise <see langword="default"/>.</returns>
    protected abstract TKey? GetOperatorId();

    /// <inheritdoc/>
    protected override sealed void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var auditOptions = GetAuditOptions();

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
        builder.ConfigureAuditEntityProperties<TUser>(auditOptions.Comments);
    }

    /// <summary>
    /// Configure the entity models.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected virtual void ConfigureModels(ModelBuilder builder)
    {
    }

    private AuditOptions GetAuditOptions()
    {
        return this.GetService<AuditOptions>();
    }
}