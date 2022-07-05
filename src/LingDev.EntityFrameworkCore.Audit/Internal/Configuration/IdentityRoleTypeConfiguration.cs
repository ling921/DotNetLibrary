using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityRoleTypeConfiguration<TRole, TRoleClaim, TKey> : IEntityTypeConfiguration<TRole>
    where TRole : IdentityRole<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TRole> builder)
    {
        builder.HasKey(r => r.Id);

        builder.ToTable("Roles")
               .HasComment("A role in the identity system.");

        builder.Property(r => r.Id)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key for this role.");

        builder.Property(r => r.Name)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The name for this role.");

        builder.Property(r => r.NormalizedName)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The normalized name for this role.");

        builder.Property(r => r.ConcurrencyStamp)
               .IsUnicode(false)
               .HasMaxLength(36)
               .IsConcurrencyToken()
               .HasComment("A random value that must change whenever a role is persisted to the store.");

        builder.HasMany<TRoleClaim>()
               .WithOne()
               .HasForeignKey(rc => rc.RoleId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
