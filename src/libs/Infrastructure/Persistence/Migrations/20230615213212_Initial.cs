using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RazaoSocial = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Cnpj = table.Column<string>(type: "VARCHAR(18)", nullable: false),
                    Cep = table.Column<string>(type: "VARCHAR(9)", maxLength: 9, nullable: false),
                    Endereco = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Bairro = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "CHAR(2)", maxLength: 2, nullable: false),
                    Numero = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateAuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lead_Id", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leads");
        }
    }
}
