using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Sections");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "FieldResponses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "FieldResponses");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Sections",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
