using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Audit log.
/// </summary>
/// <typeparam name="TUser">The type of operator.</typeparam>
[DisableAuditing]
public class AuditLog<TUser> : Entity<long>
    where TUser : class
{
    /// <summary>
    /// The primary key of changed entity.
    /// </summary>
    public string EntityId { get; set; } = null!;

    /// <summary>
    /// The type of changed entity.
    /// </summary>
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// The type of audit event.
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// The time the audit event occurred.
    /// </summary>
    public DateTimeOffset EventTime { get; set; }

    /// <summary>
    /// The user who change entity.
    /// </summary>
    public TUser? Operator { get; set; }

    /// <summary>
    /// The details of this audit log.
    /// </summary>
    public virtual ICollection<AuditLogDetail> Details { get; set; } = new List<AuditLogDetail>();
}
