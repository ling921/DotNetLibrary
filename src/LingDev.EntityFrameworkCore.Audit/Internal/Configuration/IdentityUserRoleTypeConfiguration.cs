using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityUserRoleTypeConfiguration<TUserRole, TKey> : IEntityTypeConfiguration<TUserRole>
    where TUserRole : IdentityUserRole<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.ToTable("UserRoles")
               .HasComment("A claim that a user possesses.");

        builder.Property(ur => ur.UserId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the user that is linked to a role.");

        builder.Property(ur => ur.RoleId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the role that is linked to the user.");
    }
}
