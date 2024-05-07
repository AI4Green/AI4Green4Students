using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdatReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_LiteratureReviews_LiteratureReviewId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Notes_NoteId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Plans_PlanId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_ProjectGroups_ProjectGroupId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Plans_PlanId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_PlanId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_OwnerId",
                table: "Reports",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_LiteratureReviews_LiteratureReviewId",
                table: "FieldResponses",
                column: "LiteratureReviewId",
                principalTable: "LiteratureReviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Notes_NoteId",
                table: "FieldResponses",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Plans_PlanId",
                table: "FieldResponses",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_ProjectGroups_ProjectGroupId",
                table: "FieldResponses",
                column: "ProjectGroupId",
                principalTable: "ProjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_OwnerId",
                table: "Reports",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_LiteratureReviews_LiteratureReviewId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Notes_NoteId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_Plans_PlanId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldResponses_ProjectGroups_ProjectGroupId",
                table: "FieldResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_OwnerId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_OwnerId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PlanId",
                table: "Reports",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_LiteratureReviews_LiteratureReviewId",
                table: "FieldResponses",
                column: "LiteratureReviewId",
                principalTable: "LiteratureReviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Notes_NoteId",
                table: "FieldResponses",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_Plans_PlanId",
                table: "FieldResponses",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldResponses_ProjectGroups_ProjectGroupId",
                table: "FieldResponses",
                column: "ProjectGroupId",
                principalTable: "ProjectGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Plans_PlanId",
                table: "Reports",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
