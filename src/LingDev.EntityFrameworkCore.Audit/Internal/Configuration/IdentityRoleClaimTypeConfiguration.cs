using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityRoleClaimTypeConfiguration<TRoleClaim, TKey> : IEntityTypeConfiguration<TRoleClaim>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.ToTable("RoleClaims")
               .HasComment("A claim that a role possesses.");

        builder.Property(rc => rc.Id)
               .HasComment("The identifier for this role claim.");

        builder.Property(rc => rc.RoleId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the role associated with this claim.");

        builder.Property(rc => rc.ClaimType)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The claim type for this claim.");

        builder.Property(rc => rc.ClaimValue)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The claim value for this claim.");
    }
}
