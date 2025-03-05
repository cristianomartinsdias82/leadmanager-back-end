using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PendingMigrationsAfterEf9Upgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Prospecting",
                table: "Leads",
                keyColumn: "Id",
                keyValue: new Guid("08dd37fe-d415-d570-0015-5d5ac0530000"));

            migrationBuilder.InsertData(
                schema: "Prospecting",
                table: "Leads",
                columns: new[] { "Id", "Bairro", "Cep", "Cidade", "Cnpj", "Complemento", "CreatedAt", "CreatedUserId", "Estado", "Endereco", "Numero", "RazaoSocial", "UpdatedAt", "UpdatedUserId" },
                values: new object[] { new Guid("0e81a1fa-976c-4f18-8802-3d315c37ac1e"), "Jardim Campinas", "04858-040", "São Paulo", "80.732.377/0001-74", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("00000000-0000-0000-0000-000000000000"), "SP", "Constelação do Escorpião", "43", "Lead Manager Brasil S.A.", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Prospecting",
                table: "Leads",
                keyColumn: "Id",
                keyValue: new Guid("0e81a1fa-976c-4f18-8802-3d315c37ac1e"));

            migrationBuilder.InsertData(
                schema: "Prospecting",
                table: "Leads",
                columns: new[] { "Id", "Bairro", "Cep", "Cidade", "Cnpj", "Complemento", "CreatedAt", "CreatedUserId", "Estado", "Endereco", "Numero", "RazaoSocial", "UpdatedAt", "UpdatedUserId" },
                values: new object[] { new Guid("08dd37fe-d415-d570-0015-5d5ac0530000"), "Jardim Campinas", "04858-040", "São Paulo", "80.732.377/0001-74", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("00000000-0000-0000-0000-000000000000"), "SP", "Constelação do Escorpião", "43", "Lead Manager Brasil S.A.", null, null });
        }
    }
}
