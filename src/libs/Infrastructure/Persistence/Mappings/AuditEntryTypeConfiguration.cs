using Application.Core;
using Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public sealed class AuditEntryTypeConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable(AuditEntryMetadata.TableName, AuditEntryMetadata.DatabaseSchemaName);

        builder.HasKey(x => x.Id)
                .HasName(AuditEntryMetadata.AuditEntryIdColumnPkId);

        builder.Property(x => x.Id)
                .ValueGeneratedNever();

        builder.Property(x => x.ActionDateTime)
               .IsRequired();

        builder.Property(x => x.UserId)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(255);

        builder.Property(x => x.Action)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(100)
               .HasConversion(new EnumToStringDescriptionConverter<SystemActions>());

        builder.Property(x => x.SubjectId);

        builder.Property(x => x.FullyQualifiedTypeName)
               .HasColumnType("VARCHAR")
               .HasMaxLength(255);

        builder.Property(x => x.OldData)
               .HasColumnType("TEXT");

        builder.Property(x => x.NewData)
               .HasColumnType("TEXT");
    }
}