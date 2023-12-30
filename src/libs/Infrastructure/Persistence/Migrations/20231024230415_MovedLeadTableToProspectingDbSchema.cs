using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovedLeadTableToProspectingDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Prospecting");

            migrationBuilder.RenameTable(
                name: "Leads",
                newName: "Leads",
                newSchema: "Prospecting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Leads",
                schema: "Prospecting",
                newName: "Leads");
        }
    }
}
