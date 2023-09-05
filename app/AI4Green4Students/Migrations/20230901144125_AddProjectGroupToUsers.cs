using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectGroupToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ProjectGroups_ProjectGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjectGroupId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserProjectGroup",
                columns: table => new
                {
                    ProjectGroupsId = table.Column<int>(type: "integer", nullable: false),
                    StudentsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserProjectGroup", x => new { x.ProjectGroupsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserProjectGroup_AspNetUsers_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserProjectGroup_ProjectGroups_ProjectGroupsId",
                        column: x => x.ProjectGroupsId,
                        principalTable: "ProjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserProjectGroup_StudentsId",
                table: "ApplicationUserProjectGroup",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserProjectGroup");

            migrationBuilder.AddColumn<int>(
                name: "ProjectGroupId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectGroupId",
                table: "AspNetUsers",
                column: "ProjectGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ProjectGroups_ProjectGroupId",
                table: "AspNetUsers",
                column: "ProjectGroupId",
                principalTable: "ProjectGroups",
                principalColumn: "Id");
        }
    }
}
