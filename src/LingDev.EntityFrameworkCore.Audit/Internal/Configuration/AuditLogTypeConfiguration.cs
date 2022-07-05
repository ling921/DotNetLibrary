using LingDev.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class AuditLogTypeConfiguration<TUser> : IEntityTypeConfiguration<AuditLog<TUser>>
    where TUser : class
{
    public void Configure(EntityTypeBuilder<AuditLog<TUser>> builder)
    {
        builder.HasKey(al => al.Id);

        builder.ToTable("AuditLogs")
               .HasComment("The change logs of changed entity.");

        builder.Property(al => al.Id)
               .HasComment("The primary key for this entity.");

        builder.Property(al => al.EntityId)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of changed entity.");

        builder.Property(al => al.EntityType)
               .IsUnicode(false)
               .HasMaxLength(64)
               .HasConversion<string>()
               .HasComment("The type of changed entity.");

        builder.Property(al => al.EventType)
               .IsUnicode(false)
               .HasMaxLength(16)
               .HasConversion<string>()
               .HasComment("The type of audit event.");

        builder.Property(al => al.EventTime)
               .HasComment("The time the audit event occurred.");

        builder.HasOne(al => al.Operator)
               .WithMany()
               .HasForeignKey("OperatorId")
               .OnDelete(DeleteBehavior.SetNull);

        builder.Property("OperatorId")
               .HasComment("The primary key of the user who change entity.");

        builder.HasMany(al => al.Details)
               .WithOne()
               .HasForeignKey(ald => ald.AuditLogId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
