using LingDev.EntityFrameworkCore.Audit.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class IdentityUserClaimTypeConfiguration<TUserClaim, TKey> : IEntityTypeConfiguration<TUserClaim>
    where TUserClaim : IdentityUserClaim<TKey>
    where TKey : IEquatable<TKey>
{
    public void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.HasKey(uc => uc.Id);

        builder.ToTable("UserClaims")
               .HasComment("A claim that a user possesses.");

        builder.Property(uc => uc.Id)
               .HasComment("The identifier for this user claim.");

        builder.Property(uc => uc.UserId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of the user associated with this claim.");

        builder.Property(uc => uc.ClaimType)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The claim type for this claim.");

        builder.Property(uc => uc.ClaimValue)
               .IsUnicode(false)
               .HasMaxLength(256)
               .HasComment("The claim value for this claim.");
    }
}
