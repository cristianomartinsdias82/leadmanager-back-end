using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFileNameColumnToGeneratedFileNameInRepGenReqTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GeneratedFileFullPath",
                schema: "Processes",
                table: "ReportGenerationRequests",
                newName: "GeneratedFileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GeneratedFileName",
                schema: "Processes",
                table: "ReportGenerationRequests",
                newName: "GeneratedFileFullPath");
        }
    }
}
