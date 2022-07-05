using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityUserTokenTypeConfiguration<TUserToken, TKey> : IEntityTypeConfiguration<TUserToken>
    where TUserToken : IdentityUserToken<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TUserToken> builder)
    {
        builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

        builder.ToTable("UserTokens")
               .HasComment("An authentication token for a user.");

        builder.Property(ut => ut.UserId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the user that the token belongs to.");

        builder.Property(ut => ut.LoginProvider)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The login provider this token is from.");

        builder.Property(ut => ut.Name)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The name of the token.");

        builder.Property(ut => ut.Value)
               .IsUnicode(false)
               .HasMaxLength(1024)
               .HasComment("The token value.");
    }
}
