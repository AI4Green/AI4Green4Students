using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_ProjectGroups_ProjectGroupId",
                table: "Plans");

            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_ProjectGroupId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "ProjectGroupId",
                table: "Plans");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Plans",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Plans",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ProjectGroupId",
                table: "Plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_ProjectGroupId",
                table: "Plans",
                column: "ProjectGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_ProjectGroups_ProjectGroupId",
                table: "Plans",
                column: "ProjectGroupId",
                principalTable: "ProjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Projects_ProjectId",
                table: "Plans",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
