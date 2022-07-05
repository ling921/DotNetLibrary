using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
[DisableAuditing]
public class IdentityUserRole<TKey> : Microsoft.AspNetCore.Identity.IdentityUserRole<TKey>, IEntity
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { UserId, RoleId };
    }
}
