using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class SectionProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Experiments_ExperimentId",
                table: "Sections");

            migrationBuilder.RenameColumn(
                name: "ExperimentId",
                table: "Sections",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_ExperimentId",
                table: "Sections",
                newName: "IX_Sections_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Projects_ProjectId",
                table: "Sections",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Projects_ProjectId",
                table: "Sections");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Sections",
                newName: "ExperimentId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_ProjectId",
                table: "Sections",
                newName: "IX_Sections_ExperimentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Experiments_ExperimentId",
                table: "Sections",
                column: "ExperimentId",
                principalTable: "Experiments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
