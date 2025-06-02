using Application.Reporting.Core;
using Infrastructure.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Results;

namespace Infrastructure.Persistence.Mappings;

public sealed class ReportGenerationRequestTypeConfiguration : IEntityTypeConfiguration<ReportGenerationRequest>
{
    public void Configure(EntityTypeBuilder<ReportGenerationRequest> builder)
    {
        builder.ToTable(ReportGenerationRequestMetadata.TableName,
						ReportGenerationRequestMetadata.DatabaseSchemaName);

        builder.HasKey(x => x.Id)
                .HasName(ReportGenerationRequestMetadata.ReportGenerationRequestColumnPkId);

		builder.Property(x => x.Id)
				.UseIdentityColumn();

		builder.Property(x => x.UserId)
			   .IsRequired()
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(255);

		builder.Property(x => x.SerializedParameters)
               .HasColumnType("VARCHAR")
               .HasMaxLength(8000);

		builder.Property(x => x.SerializedParametersDataType)
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(255);

		builder.Property(x => x.Feature)
			   .IsRequired()
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(100)
			   .HasConversion(new EnumToStringDescriptionConverter<ReportGenerationFeatures>());

		builder.Property(x => x.Status)
			   .IsRequired()
			   .HasColumnType("VARCHAR")
			   .HasMaxLength(40)
			   .HasConversion(new EnumToStringDescriptionConverter<OperationStatuses>());
	}
}