using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProjectGroupNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectGroups_Name",
                table: "ProjectGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectGroups_Name",
                table: "ProjectGroups",
                column: "Name",
                unique: true);
        }
    }
}
