using LingDev.EntityFrameworkCore.Audit.Attributes;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
[DisableAuditing]
public class IdentityUserToken<TKey> : Microsoft.AspNetCore.Identity.IdentityUserToken<TKey>, IEntity
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { UserId, LoginProvider, Name };
    }
}
