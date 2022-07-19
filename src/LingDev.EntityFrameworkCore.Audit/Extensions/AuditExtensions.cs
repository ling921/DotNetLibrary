using LingDev.EntityFrameworkCore.Audit;
using LingDev.EntityFrameworkCore.Audit.Identity;
using LingDev.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for Dependency Injection.
/// </summary>
public static class AuditExtensions
{
    /// <summary>
    /// Registers the given <typeparamref name="TDbContext"/> as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="optionsAction">
    /// An optional action to configure the <see cref="DbContextOptions"/> for the context. This
    /// provides an alternative to performing configuration of the context by overriding the <see
    /// cref="DbContext.OnConfiguring"/> method in your derived context.
    /// </param>
    /// <param name="setupAcion">The action used to configure the <see cref="AuditOptions"/>.</param>
    /// <param name="contextLifetime">
    /// The lifetime with which to register the DbContext service in the container.
    /// </param>
    /// <param name="optionsLifetime">
    /// The lifetime with which to register the DbContextOptions service in the container.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddAuditDbContext<TDbContext, TUser>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        Action<AuditOptions>? setupAcion = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext, IAuditDbContext<TUser>
        where TUser : class, IEntity
    {
        services.ConfigureAuditOptions(setupAcion);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }

    /// <summary>
    /// Registers the given <see cref="IdentityAuditDbContext{TDbContext}"/> as a service in the
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="optionsAction">
    /// An optional action to configure the <see cref="DbContextOptions"/> for the context. This
    /// provides an alternative to performing configuration of the context by overriding the <see
    /// cref="DbContext.OnConfiguring"/> method in your derived context.
    /// </param>
    /// <param name="setupAcion">The action used to configure the <see cref="AuditOptions"/>.</param>
    /// <param name="contextLifetime">
    /// The lifetime with which to register the DbContext service in the container.
    /// </param>
    /// <param name="optionsLifetime">
    /// The lifetime with which to register the DbContextOptions service in the container.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddIdentityAuditDbContext<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        Action<AuditOptions>? setupAcion = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : IdentityAuditDbContext<TDbContext>
    {
        services.ConfigureAuditOptions(setupAcion);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }

    /// <summary>
    /// Registers the given <see cref="IdentityAuditDbContext{TDbContext, TUser, TKey}"/> as a
    /// service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="optionsAction">
    /// An optional action to configure the <see cref="DbContextOptions"/> for the context. This
    /// provides an alternative to performing configuration of the context by overriding the <see
    /// cref="DbContext.OnConfiguring"/> method in your derived context.
    /// </param>
    /// <param name="setupAcion">The action used to configure the <see cref="AuditOptions"/>.</param>
    /// <param name="contextLifetime">
    /// The lifetime with which to register the DbContext service in the container.
    /// </param>
    /// <param name="optionsLifetime">
    /// The lifetime with which to register the DbContextOptions service in the container.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddIdentityAuditDbContext<TDbContext, TUser, TKey>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        Action<AuditOptions>? setupAcion = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : IdentityAuditDbContext<TDbContext, TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        services.ConfigureAuditOptions(setupAcion);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }

    /// <summary>
    /// Registers the given <see cref="IdentityAuditDbContext{TDbContext, TUser, TRole, TKey}"/> as
    /// a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> object.</typeparam>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TRole">The type of role objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="optionsAction">
    /// An optional action to configure the <see cref="DbContextOptions"/> for the context. This
    /// provides an alternative to performing configuration of the context by overriding the <see
    /// cref="DbContext.OnConfiguring"/> method in your derived context.
    /// </param>
    /// <param name="setupAcion">The action used to configure the <see cref="AuditOptions"/>.</param>
    /// <param name="contextLifetime">
    /// The lifetime with which to register the DbContext service in the container.
    /// </param>
    /// <param name="optionsLifetime">
    /// The lifetime with which to register the DbContextOptions service in the container.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddIdentityAuditDbContext<TDbContext, TUser, TRole, TKey>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        Action<AuditOptions>? setupAcion = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : IdentityAuditDbContext<TDbContext, TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        services.ConfigureAuditOptions(setupAcion);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }

    /// <summary>
    /// Registers the given <see cref="IdentityAuditDbContext{TDbContext, TUser, TRole, TKey,
    /// TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> as a service in the <see cref="IServiceCollection"/>.
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
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="optionsAction">
    /// An optional action to configure the <see cref="DbContextOptions"/> for the context. This
    /// provides an alternative to performing configuration of the context by overriding the <see
    /// cref="DbContext.OnConfiguring"/> method in your derived context.
    /// </param>
    /// <param name="setupAcion">The action used to configure the <see cref="AuditOptions"/>.</param>
    /// <param name="contextLifetime">
    /// The lifetime with which to register the DbContext service in the container.
    /// </param>
    /// <param name="optionsLifetime">
    /// The lifetime with which to register the DbContextOptions service in the container.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddIdentityAuditDbContext<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        Action<AuditOptions>? setupAcion = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContext : IdentityAuditDbContext<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        services.ConfigureAuditOptions(setupAcion);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }

    private static void ConfigureAuditOptions(this IServiceCollection services, Action<AuditOptions>? setupAcion)
    {
        if (setupAcion == null)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AuditOptions>, AuditConfigureOptions>());
        }
        else
        {
            services.Configure(setupAcion);
        }
    }
}
