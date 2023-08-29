using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectToProjectGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectGroups_Projects_ProjectId",
                table: "ProjectGroups");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectGroups_Name",
                table: "ProjectGroups",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectGroups_Projects_ProjectId",
                table: "ProjectGroups",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectGroups_Projects_ProjectId",
                table: "ProjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Name",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectGroups_Name",
                table: "ProjectGroups");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectGroups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectGroups_Projects_ProjectId",
                table: "ProjectGroups",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
