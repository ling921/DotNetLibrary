using LingDev.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingDev.EntityFrameworkCore.Internal.Configuration;

internal class AuditLogDetailTypeConfiguration : IEntityTypeConfiguration<AuditLogDetail>
{
    public void Configure(EntityTypeBuilder<AuditLogDetail> builder)
    {
        builder.HasKey(al => al.Id);

        builder.ToTable("AuditLogDetails")
               .HasComment("The property change logs of changed entity.");

        builder.Property(al => al.Id)
               .HasComment("The primary key of changed entity.");

        builder.Property(al => al.AuditLogId)
               .HasComment("The primary key of the AuditLog that the detail belongs to.");

        builder.Property(al => al.PropertyName)
               .IsUnicode(false)
               .HasMaxLength(64)
               .HasComment("The property name (format: entity class name + '.' + property name).");

        builder.Property(al => al.OriginalValue)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The original value, more than 128 characters will be truncated.");

        builder.Property(al => al.NewValue)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The new value, more than 128 characters will be truncated.");
    }
}
