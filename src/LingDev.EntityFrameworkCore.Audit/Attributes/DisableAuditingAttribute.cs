namespace LingDev.EntityFrameworkCore.Audit.Attributes;

/// <summary>
/// Indicates the class or property is marked as not auditable.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class DisableAuditingAttribute : Attribute
{
}
