using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityUserLoginTypeConfiguration<TUserLogin, TKey> : IEntityTypeConfiguration<TUserLogin>
    where TUserLogin : IdentityUserLogin<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TUserLogin> builder)
    {
        builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

        builder.ToTable("UserLogins")
               .HasComment("A login and its associated provider for a user.");

        builder.Property(ul => ul.LoginProvider)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The login provider for the login (e.g. facebook, google).");

        builder.Property(ul => ul.ProviderKey)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The unique provider identifier for this login.");

        builder.Property(ul => ul.ProviderDisplayName)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The friendly name used in a UI for this login.");

        builder.Property(ul => ul.UserId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the user associated with this login.");
    }
}
