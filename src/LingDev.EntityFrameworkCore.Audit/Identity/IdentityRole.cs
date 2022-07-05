using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Identity;

/// <inheritdoc/>
public class IdentityRole<TKey> : Microsoft.AspNetCore.Identity.IdentityRole<TKey>, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc/>
    public object?[] GetKeys()
    {
        return new object?[] { Id };
    }
}
