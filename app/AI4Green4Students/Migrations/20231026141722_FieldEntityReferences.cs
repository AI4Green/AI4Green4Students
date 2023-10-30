using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class FieldEntityReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Sections_SectionId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_FieldResponseId",
                table: "Conversations");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Sections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SectionId",
                table: "Fields",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_FieldResponseId",
                table: "Conversations",
                column: "FieldResponseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Sections_SectionId",
                table: "Fields",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Sections_SectionId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_FieldResponseId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Sections");

            migrationBuilder.AlterColumn<int>(
                name: "SectionId",
                table: "Fields",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_FieldResponseId",
                table: "Conversations",
                column: "FieldResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Sections_SectionId",
                table: "Fields",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id");
        }
    }
}
