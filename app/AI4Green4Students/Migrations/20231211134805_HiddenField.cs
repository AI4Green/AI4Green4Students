using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class HiddenField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectFieldOption_Fields_FieldId",
                table: "SelectFieldOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectFieldOption",
                table: "SelectFieldOption");

            migrationBuilder.RenameTable(
                name: "SelectFieldOption",
                newName: "SelectFieldOptions");

            migrationBuilder.RenameIndex(
                name: "IX_SelectFieldOption_FieldId",
                table: "SelectFieldOptions",
                newName: "IX_SelectFieldOptions_FieldId");

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "Fields",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectFieldOptions",
                table: "SelectFieldOptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectFieldOptions_Fields_FieldId",
                table: "SelectFieldOptions",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectFieldOptions_Fields_FieldId",
                table: "SelectFieldOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectFieldOptions",
                table: "SelectFieldOptions");

            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "Fields");

            migrationBuilder.RenameTable(
                name: "SelectFieldOptions",
                newName: "SelectFieldOption");

            migrationBuilder.RenameIndex(
                name: "IX_SelectFieldOptions_FieldId",
                table: "SelectFieldOption",
                newName: "IX_SelectFieldOption_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectFieldOption",
                table: "SelectFieldOption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectFieldOption_Fields_FieldId",
                table: "SelectFieldOption",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
