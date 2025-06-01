using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportGenerationRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Processes");

            migrationBuilder.CreateTable(
                name: "ReportGenerationRequests",
                schema: "Processes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastProcessedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Feature = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    SerializedParameters = table.Column<string>(type: "VARCHAR(8000)", maxLength: 8000, nullable: true),
                    SerializedParametersDataType = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    ExecutionAttempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGenerationRequest_Id", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportGenerationRequests",
                schema: "Processes");
        }
    }
}
