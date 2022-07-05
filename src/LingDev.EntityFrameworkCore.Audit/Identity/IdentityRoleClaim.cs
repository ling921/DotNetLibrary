using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
[DisableAuditing]
public class IdentityRoleClaim<TKey> : Microsoft.AspNetCore.Identity.IdentityRoleClaim<TKey>, IEntity<int>
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { Id };
    }
}
