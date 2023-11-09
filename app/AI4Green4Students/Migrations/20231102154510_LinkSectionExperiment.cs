using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class LinkSectionExperiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_AspNetUsers_StudentId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_StudentId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "FieldResponses");

            migrationBuilder.AddColumn<int>(
                name: "ExperimentId",
                table: "FieldResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_ExperimentId",
                table: "FieldResponses",
                column: "ExperimentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Experiments_ExperimentId",
                table: "FieldResponses",
                column: "ExperimentId",
                principalTable: "Experiments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Experiments_ExperimentId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_ExperimentId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "ExperimentId",
                table: "FieldResponses");

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "FieldResponses",
                type: "text",
                nullable: true);
      
            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_StudentId",
                table: "FieldResponses",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_AspNetUsers_StudentId",
                table: "FieldResponses",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
