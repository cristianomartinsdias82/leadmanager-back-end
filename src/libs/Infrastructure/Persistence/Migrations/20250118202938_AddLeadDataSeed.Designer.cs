﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(LeadManagerDbContext))]
    [Migration("20250118202938_AddLeadDataSeed")]
    partial class AddLeadDataSeed
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Prospecting")
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Application.Core.AuditEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("VARCHAR");

                    b.Property<DateTimeOffset>("ActionDateTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("FullyQualifiedTypeName")
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("NewData")
                        .HasColumnType("TEXT");

                    b.Property<string>("OldData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SubjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR");

                    b.HasKey("Id")
                        .HasName("PK_AuditEntry_Id");

                    b.ToTable("AuditEntries", "Auditing");
                });

            modelBuilder.Entity("Domain.Prospecting.Entities.Lead", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Complemento")
                        .HasMaxLength(50)
                        .HasColumnType("VARCHAR");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("CreatedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("CHAR");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("Endereco");

                    b.Property<string>("Numero")
                        .HasMaxLength(20)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("RazaoSocial")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("VARCHAR");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("UpdatedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id")
                        .HasName("PK_Lead_Id");

                    b.HasIndex(new[] { "Cnpj" }, "IX_Leads_Cnpj")
                        .IsUnique();

                    b.HasIndex(new[] { "RazaoSocial" }, "IX_Leads_RazaoSocial")
                        .IsUnique();

                    b.ToTable("Leads", "Prospecting");

                    b.HasData(
                        new
                        {
                            Id = new Guid("08dd37fe-d415-d570-0015-5d5ac0530000"),
                            Bairro = "Jardim Campinas",
                            Cep = "04858-040",
                            Cidade = "São Paulo",
                            Cnpj = "80.732.377/0001-74",
                            CreatedAt = new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CreatedUserId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Estado = "SP",
                            Logradouro = "Constelação do Escorpião",
                            Numero = "43",
                            RazaoSocial = "Lead Manager Brasil S.A.",
                            RowVersion = new byte[] { 254, 55, 221, 8, 21, 212, 112, 213, 0, 21, 93, 90, 192, 83, 0, 0 }
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
