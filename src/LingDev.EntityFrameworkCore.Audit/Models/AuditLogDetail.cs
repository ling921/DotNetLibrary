using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Detail of <see cref="AuditLog{TUserKey}"/>.
/// </summary>
[DisableAuditing]
public class AuditLogDetail : Entity<long>
{
    /// <summary>
    /// The primary key of the AuditLog that the detail belongs to.
    /// </summary>
    public long AuditLogId { get; set; }

    /// <summary>
    /// Property name.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Original value, more than 128 characters will be truncated.
    /// </summary>
    public string? OriginalValue { get; set; }

    /// <summary>
    /// New value, more than 128 characters will be truncated.
    /// </summary>
    public string? NewValue { get; set; }
}
