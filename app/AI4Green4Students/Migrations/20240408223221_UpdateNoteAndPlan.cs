using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNoteAndPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Plans_PlanId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteFieldResponse_FieldResponses_FieldResponseId",
                table: "NoteFieldResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteFieldResponse_Note_NoteId",
                table: "NoteFieldResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteFieldResponse",
                table: "NoteFieldResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Note",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_PlanId",
                table: "Note");

            migrationBuilder.RenameTable(
                name: "NoteFieldResponse",
                newName: "NoteFieldResponses");

            migrationBuilder.RenameTable(
                name: "Note",
                newName: "Notes");

            migrationBuilder.RenameIndex(
                name: "IX_NoteFieldResponse_FieldResponseId",
                table: "NoteFieldResponses",
                newName: "IX_NoteFieldResponses_FieldResponseId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Plans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteFieldResponses",
                table: "NoteFieldResponses",
                columns: new[] { "NoteId", "FieldResponseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_PlanId",
                table: "Notes",
                column: "PlanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteFieldResponses_FieldResponses_FieldResponseId",
                table: "NoteFieldResponses",
                column: "FieldResponseId",
                principalTable: "FieldResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteFieldResponses_Notes_NoteId",
                table: "NoteFieldResponses",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Plans_PlanId",
                table: "Notes",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteFieldResponses_FieldResponses_FieldResponseId",
                table: "NoteFieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteFieldResponses_Notes_NoteId",
                table: "NoteFieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Plans_PlanId",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_PlanId",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteFieldResponses",
                table: "NoteFieldResponses");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Plans");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "Note");

            migrationBuilder.RenameTable(
                name: "NoteFieldResponses",
                newName: "NoteFieldResponse");

            migrationBuilder.RenameIndex(
                name: "IX_NoteFieldResponses_FieldResponseId",
                table: "NoteFieldResponse",
                newName: "IX_NoteFieldResponse_FieldResponseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Note",
                table: "Note",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteFieldResponse",
                table: "NoteFieldResponse",
                columns: new[] { "NoteId", "FieldResponseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Note_PlanId",
                table: "Note",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Plans_PlanId",
                table: "Note",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteFieldResponse_FieldResponses_FieldResponseId",
                table: "NoteFieldResponse",
                column: "FieldResponseId",
                principalTable: "FieldResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteFieldResponse_Note_NoteId",
                table: "NoteFieldResponse",
                column: "NoteId",
                principalTable: "Note",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
