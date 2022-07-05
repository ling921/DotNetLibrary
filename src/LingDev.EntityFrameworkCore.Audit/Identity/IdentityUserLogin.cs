using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
[DisableAuditing]
public class IdentityUserLogin<TKey> : Microsoft.AspNetCore.Identity.IdentityUserLogin<TKey>, IEntity
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { LoginProvider, ProviderKey };
    }
}
