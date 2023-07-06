using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndexesToCnpjRazaoSocialColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Leads_Cnpj",
                table: "Leads",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_RazaoSocial",
                table: "Leads",
                column: "RazaoSocial",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leads_Cnpj",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_RazaoSocial",
                table: "Leads");
        }
    }
}
