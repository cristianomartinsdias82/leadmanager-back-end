using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixLeadsFilesTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "LeadsFile",
                schema: "Prospecting",
                newName: "LeadsFiles",
                newSchema: "Prospecting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "LeadsFiles",
                schema: "Prospecting",
                newName: "LeadsFile",
                newSchema: "Prospecting");
        }
    }
}
