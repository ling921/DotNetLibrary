using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
[DisableAuditing]
public class IdentityUserClaim<TKey> : Microsoft.AspNetCore.Identity.IdentityUserClaim<TKey>, IEntity<int>
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { Id };
    }
}
