using Domain.Prospecting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

internal sealed class LeadEntityTypeConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable(LeadEntityMetadata.TableName);

        builder.HasKey(x => x.Id)
                .HasName(LeadEntityMetadata.LeadIdColumnPkId);

        builder.Property(x => x.Id)
                .ValueGeneratedNever();

        //https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/concurrency?view=aspnetcore-7.0
        //https://www.learnentityframeworkcore5.com/handling-concurrency-in-ef-core
        builder.Property(x => x.RowVersion)
                .IsRowVersion() //Concurrency checking purposes
                .ValueGeneratedOnAddOrUpdate(); 

        builder.Property(x => x.Cnpj)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(18);

        builder.Property(x => x.RazaoSocial)
               .IsRequired()
               .HasColumnType("VARCHAR")
               .HasMaxLength(100);

        builder.Property(x => x.Logradouro)
               .HasColumnName("Endereco")
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

        Seed(builder);
	}

	private void Seed(EntityTypeBuilder<Lead> builder)
	{
        var lead = Lead.Criar(
            "80.732.377/0001-74",
            "Lead Manager Brasil S.A.",
            "04858-040",
            "Constelação do Escorpião",
            "Jardim Campinas",
            "São Paulo",
            "SP",
            "43",
            default);
        lead.Id = Guid.Parse("0E81A1FA-976C-4F18-8802-3D315C37AC1E");

		//Data seed
		builder.HasData(lead);
	}
}