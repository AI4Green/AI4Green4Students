using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExperiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiteratureFileLocation",
                table: "Experiments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LiteratureFileName",
                table: "Experiments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LiteratureReviewDescription",
                table: "Experiments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiteratureFileLocation",
                table: "Experiments");

            migrationBuilder.DropColumn(
                name: "LiteratureFileName",
                table: "Experiments");

            migrationBuilder.DropColumn(
                name: "LiteratureReviewDescription",
                table: "Experiments");
        }
    }
}
