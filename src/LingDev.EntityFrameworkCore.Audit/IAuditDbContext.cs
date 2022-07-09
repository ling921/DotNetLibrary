using LingDev.EntityFrameworkCore.Audit.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LingDev.EntityFrameworkCore.Audit;

/// <summary>
/// Interface for auditing <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
/// </summary>
public interface IAuditDbContext<TUser> where TUser : class
{
    /// <summary>
    /// The logger.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// The instance this <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
    /// </summary>
    DbContext DbContext { get; }

    /// <summary>
    /// Get the current operator.
    /// </summary>
    TUser? Operator { get; }

    /// <summary>
    /// Options to configure the behavior of audited entities.
    /// </summary>
    AuditOptions Options { get; }
}
