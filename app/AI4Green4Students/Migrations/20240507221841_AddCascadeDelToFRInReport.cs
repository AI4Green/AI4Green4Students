using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDelToFRInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id");
        }
    }
}
