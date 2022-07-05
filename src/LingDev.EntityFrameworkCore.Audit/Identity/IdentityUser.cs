using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
public class IdentityUser<TKey> : Microsoft.AspNetCore.Identity.IdentityUser<TKey>, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { Id };
    }
}
