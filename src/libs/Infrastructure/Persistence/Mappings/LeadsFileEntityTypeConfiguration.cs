using Domain.Prospecting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

internal sealed class LeadsFileEntityTypeConfiguration : IEntityTypeConfiguration<LeadsFile>
{
    public void Configure(EntityTypeBuilder<LeadsFile> builder)
    {
        builder.ToTable(LeadsFileEntityMetadata.TableName);

        builder.HasKey(x => x.Id)
                .HasName(LeadsFileEntityMetadata.LeadsFileIdColumnPkId);

        builder.Property(x => x.Id)
                .ValueGeneratedNever();

		builder.Property(x => x.UserId)
			   .IsRequired()
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(255);

		builder.Property(x => x.FileId)
			   .IsRequired()
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(255);

		builder.Property(x => x.FileName)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(255);

        builder.Property(x => x.Description)
               .HasColumnType("VARCHAR")
               .HasMaxLength(255);

        builder.Property(x => x.PathOrContainerName)
               .HasColumnType("VARCHAR")
               .HasMaxLength(255);
	}
}