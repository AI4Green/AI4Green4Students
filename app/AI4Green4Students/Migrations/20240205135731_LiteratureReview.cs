using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class LiteratureReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiteratureReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: true),
                    Deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiteratureReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiteratureReviews_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LiteratureReviews_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiteratureReviews_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReviewFieldResponses_FieldResponseId",
                table: "LiteratureReviewFieldResponses",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReviews_OwnerId",
                table: "LiteratureReviews",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReviews_ProjectId",
                table: "LiteratureReviews",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LiteratureReviews_StageId",
                table: "LiteratureReviews",
                column: "StageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiteratureReviewFieldResponses");

            migrationBuilder.DropTable(
                name: "LiteratureReviews");
        }
    }
}
