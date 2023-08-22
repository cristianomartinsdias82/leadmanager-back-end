using Core.Entities;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public static class LeadEntityMetadata
{
    public static string CnpjColumnIndexName = "IX_Leads_Cnpj";
    public static string RazaoSocialColumnIndexName = "IX_Leads_RazaoSocial";
}

internal sealed class LeadEntityTypeConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(x => x.Id)
                .HasName($"PK_{nameof(Lead)}_{nameof(Lead.Id)}");

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Cnpj)
               .IsRequired()
               .HasColumnType("VARCHAR(18)");

        builder.Property(x => x.RazaoSocial)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(100);

        builder.Property(x => x.Logradouro)
               .HasColumnName(nameof(Endereco))
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(100);

        builder.Property(x => x.Cep)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(9);

        builder.Property(x => x.Numero)
               .HasColumnType("VARCHAR")
               .HasMaxLength(20);

        builder.Property(x => x.Complemento)
               .HasColumnType("VARCHAR")
               .HasMaxLength(50);

        builder.Property(x => x.Bairro)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(50);

        builder.Property(x => x.Cidade)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(50);

        builder.Property(x => x.Estado)
               .IsRequired()
               .HasColumnType("CHAR")
               .HasMaxLength(2);
    }
}