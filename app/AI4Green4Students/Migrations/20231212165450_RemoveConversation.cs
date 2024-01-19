using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Conversations_ConversationId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ConversationId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "FieldResponseId",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FieldResponseId",
                table: "Comments",
                column: "FieldResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OwnerId",
                table: "Comments",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_OwnerId",
                table: "Comments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_FieldResponses_FieldResponseId",
                table: "Comments",
                column: "FieldResponseId",
                principalTable: "FieldResponses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_OwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_FieldResponses_FieldResponseId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_FieldResponseId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_OwnerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "FieldResponseId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldResponseId = table.Column<int>(type: "integer", nullable: false),
                    InstructorId = table.Column<string>(type: "text", nullable: true),
                    Resolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_FieldResponses_FieldResponseId",
                        column: x => x.FieldResponseId,
                        principalTable: "FieldResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ConversationId",
                table: "Comments",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_FieldResponseId",
                table: "Conversations",
                column: "FieldResponseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_InstructorId",
                table: "Conversations",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Conversations_ConversationId",
                table: "Comments",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
