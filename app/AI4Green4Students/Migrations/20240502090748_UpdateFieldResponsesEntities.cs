using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldResponsesEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LiteratureReviewId",
                table: "FieldResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "FieldResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "FieldResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectGroupId",
                table: "FieldResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "FieldResponses",
                type: "integer",
                nullable: true);
            
            // Migrate data from old tables to new FieldResponses table
            // Migrate data from LiteratureReviewFieldResponses to FieldResponses
            migrationBuilder.Sql(@"
                UPDATE ""FieldResponses"" AS fr
                SET ""LiteratureReviewId"" = l.""LiteratureReviewId""
                FROM ""LiteratureReviewFieldResponses"" AS l
                WHERE fr.""Id"" = l.""FieldResponseId""
            ");
  
            // Migrate data from NoteFieldResponses to FieldResponses
            migrationBuilder.Sql(@"
                UPDATE ""FieldResponses"" AS fr
                SET ""NoteId"" = n.""NoteId""
                FROM ""NoteFieldResponses"" AS n
                WHERE fr.""Id"" = n.""FieldResponseId""
            ");
  
            // Migrate data from PlanFieldResponses to FieldResponses
            migrationBuilder.Sql(@"
                UPDATE ""FieldResponses"" AS fr
                SET ""PlanId"" = p.""PlanId""
                FROM ""PlanFieldResponses"" AS p
                WHERE fr.""Id"" = p.""FieldResponseId""
            ");
  
            // Migrate data from ProjectGroupFieldResponses to FieldResponses
            migrationBuilder.Sql(@"
                UPDATE ""FieldResponses"" AS fr
                SET ""ProjectGroupId"" = pg.""ProjectGroupId""
                FROM ""ProjectGroupFieldResponses"" AS pg
                WHERE fr.""Id"" = pg.""FieldResponseId""
            ");
  
            // Migrate data from ReportFieldResponses to FieldResponses
            migrationBuilder.Sql(@"
                UPDATE ""FieldResponses"" AS fr
                SET ""ReportId"" = r.""ReportId""
                FROM ""ReportFieldResponses"" AS r
                WHERE fr.""Id"" = r.""FieldResponseId""
            ");
            
            // Drop old tables after migrating data
            migrationBuilder.DropTable(
                name: "LiteratureReviewFieldResponses");

            migrationBuilder.DropTable(
                name: "NoteFieldResponses");

            migrationBuilder.DropTable(
                name: "PlanFieldResponses");

            migrationBuilder.DropTable(
                name: "ProjectGroupFieldResponses");

            migrationBuilder.DropTable(
                name: "ReportFieldResponses");

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_LiteratureReviewId",
                table: "FieldResponses",
                column: "LiteratureReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_NoteId",
                table: "FieldResponses",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_PlanId",
                table: "FieldResponses",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_ProjectGroupId",
                table: "FieldResponses",
                column: "ProjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldResponses_ReportId",
                table: "FieldResponses",
                column: "ReportId");

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
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiteratureReviewFieldResponses",
                columns: table => new
                {
                    LiteratureReviewId = table.Column<int>(type: "integer", nullable: false),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiteratureReviewFieldResponses", x => new { x.LiteratureReviewId, x.FieldResponseId });
                    table.ForeignKey(
                        name: "FK_LiteratureReviewFieldResponses_FieldResponses_FieldResponse~",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiteratureReviewFieldResponses_LiteratureReviews_Literature~",
                        column: x => x.LiteratureReviewId,
                        principalTable: "LiteratureReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteFieldResponses",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "integer", nullable: false),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteFieldResponses", x => new { x.NoteId, x.FieldResponseId });
                    table.ForeignKey(
                        name: "FK_NoteFieldResponses_FieldResponses_FieldResponseId",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteFieldResponses_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanFieldResponses",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFieldResponses", x => new { x.PlanId, x.FieldResponseId });
                    table.ForeignKey(
                        name: "FK_PlanFieldResponses_FieldResponses_FieldResponseId",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanFieldResponses_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectGroupFieldResponses",
                columns: table => new
                {
                    ProjectGroupId = table.Column<int>(type: "integer", nullable: false),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGroupFieldResponses", x => new { x.ProjectGroupId, x.FieldResponseId });
                    table.ForeignKey(
                        name: "FK_ProjectGroupFieldResponses_FieldResponses_FieldResponseId",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectGroupFieldResponses_ProjectGroups_ProjectGroupId",
                        column: x => x.ProjectGroupId,
                        principalTable: "ProjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportFieldResponses",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "integer", nullable: false),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFieldResponses", x => new { x.ReportId, x.FieldResponseId });
                    table.ForeignKey(
                        name: "FK_ReportFieldResponses_FieldResponses_FieldResponseId",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportFieldResponses_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReviewFieldResponses_FieldResponseId",
                table: "LiteratureReviewFieldResponses",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteFieldResponses_FieldResponseId",
                table: "NoteFieldResponses",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFieldResponses_FieldResponseId",
                table: "PlanFieldResponses",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectGroupFieldResponses_FieldResponseId",
                table: "ProjectGroupFieldResponses",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFieldResponses_FieldResponseId",
                table: "ReportFieldResponses",
                column: "FieldResponseId");
            
            // Move data back to original tables from FieldResponses
            migrationBuilder.Sql(@"
                INSERT INTO ""LiteratureReviewFieldResponses"" (""LiteratureReviewId"", ""FieldResponseId"")
                SELECT fr.""LiteratureReviewId"", fr.""Id""
                FROM ""FieldResponses"" AS fr
                WHERE fr.""LiteratureReviewId"" IS NOT NULL
            ");
        
            migrationBuilder.Sql(@"
                INSERT INTO ""NoteFieldResponses"" (""NoteId"", ""FieldResponseId"")
                SELECT fr.""NoteId"", fr.""Id""
                FROM ""FieldResponses"" AS fr
                WHERE fr.""NoteId"" IS NOT NULL
            ");
        
            migrationBuilder.Sql(@"
                INSERT INTO ""PlanFieldResponses"" (""PlanId"", ""FieldResponseId"")
                SELECT fr.""PlanId"", fr.""Id""
                FROM ""FieldResponses"" AS fr
                WHERE fr.""PlanId"" IS NOT NULL
            ");
        
            migrationBuilder.Sql(@"
                INSERT INTO ""ProjectGroupFieldResponses"" (""ProjectGroupId"", ""FieldResponseId"")
                SELECT fr.""ProjectGroupId"", fr.""Id""
                FROM ""FieldResponses"" AS fr
                WHERE fr.""ProjectGroupId"" IS NOT NULL
            ");
        
            migrationBuilder.Sql(@"
                INSERT INTO ""ReportFieldResponses"" (""ReportId"", ""FieldResponseId"")
                SELECT fr.""ReportId"", fr.""Id""
                FROM ""FieldResponses"" AS fr
                WHERE fr.""ReportId"" IS NOT NULL
            ");
            
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
                name: "FK_FieldResponses_Reports_ReportId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_LiteratureReviewId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_NoteId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_PlanId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_ProjectGroupId",
                table: "FieldResponses");

            migrationBuilder.DropIndex(
                name: "IX_FieldResponses_ReportId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "LiteratureReviewId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "ProjectGroupId",
                table: "FieldResponses");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "FieldResponses");
        }
    }
}
