using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityUserTypeConfiguration<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> : IEntityTypeConfiguration<TUser>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TUserToken : IdentityUserToken<TKey>
{
    public void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.NormalizedUserName)
               .HasDatabaseName("UserNameIndex")
               .IsUnique();

        builder.HasIndex(u => u.NormalizedEmail)
               .HasDatabaseName("EmailIndex");

        builder.ToTable("Users")
               .HasComment("A user in the identity system.");

        builder.Property(u => u.Id)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key for this user.");

        builder.Property(u => u.UserName)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The user name for this user.");

        builder.Property(u => u.NormalizedUserName)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The normalized user name for this user.");

        builder.Property(u => u.Email)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The email for this user.");

        builder.Property(u => u.NormalizedEmail)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The normalized email for this user.");

        builder.Property(u => u.EmailConfirmed)
               .HasComment("A flag indicating if a user has confirmed their email address.");

        builder.Property(u => u.PasswordHash)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("A salted and hashed representation of the password for this user.");

        builder.Property(u => u.SecurityStamp)
               .IsUnicode(false)
               .HasMaxLength(36)
               .HasComment("A random value that must change whenever a users credentials change (password changed, login removed).");

        builder.Property(u => u.ConcurrencyStamp)
               .IsUnicode(false)
               .HasMaxLength(36)
               .IsConcurrencyToken()
               .HasComment("A random value that must change whenever a user is persisted to the store.");

        builder.Property(u => u.PhoneNumber)
               .IsUnicode(false)
               .HasMaxLength(32)
               .HasComment("A telephone number for the user.");

        builder.Property(u => u.PhoneNumberConfirmed)
               .HasComment("A flag indicating if a user has confirmed their telephone number.");

        builder.Property(u => u.TwoFactorEnabled)
               .HasComment("A flag indicating if two factor authentication is enabled for this user.");

        builder.Property(u => u.LockoutEnd)
               .HasComment("The date and time, in UTC, when any user lockout ends.");

        builder.Property(u => u.LockoutEnabled)
               .HasComment("A flag indicating if the user could be locked out.");

        builder.Property(u => u.AccessFailedCount)
               .HasComment("The number of failed login attempts for the current user.");

        builder.HasMany<TUserClaim>()
               .WithOne()
               .HasForeignKey(uc => uc.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TUserRole>()
               .WithOne()
               .HasForeignKey(ur => ur.RoleId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TUserLogin>()
               .WithOne()
               .HasForeignKey(ul => ul.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<TUserToken>()
               .WithOne()
               .HasForeignKey(ut => ut.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
