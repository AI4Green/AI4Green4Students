using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectGroupAndNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperimentDeadline",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PlanningDeadline",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Projects");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExperimentDeadline",
                table: "ProjectGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PlanningDeadline",
                table: "ProjectGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "ProjectGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deadline",
                table: "Notes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Notes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StageId",
                table: "Notes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            // Update existing Notes with valid OwnerId and ProjectId using Plan as its reference
            // Also, update existing Notes with valid StageId (default to Draft)
            migrationBuilder.Sql(@"
                UPDATE ""Notes"" 
                SET ""OwnerId"" = (
                    SELECT ""OwnerId"" 
                    FROM ""Plans"" 
                    WHERE ""Plans"".""Id"" = ""Notes"".""PlanId""
                ) 
                WHERE ""OwnerId"" IS NULL;
            
                UPDATE ""Notes"" 
                SET ""ProjectId"" = (
                    SELECT ""ProjectId"" 
                    FROM ""Plans"" 
                    WHERE ""Plans"".""Id"" = ""Notes"".""PlanId""
                ) 
                WHERE ""ProjectId"" = 0;
            
                UPDATE ""Notes"" 
                SET ""StageId"" = (
                    SELECT ""Id"" 
                    FROM ""Stages"" 
                    WHERE ""DisplayName"" = 'Draft' 
                      AND ""TypeId"" = (
                        SELECT ""Id"" 
                        FROM ""StageTypes"" 
                        WHERE ""Value"" = 'Note'
                      )
                ) 
                WHERE ""StageId"" = 0;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ProjectId",
                table: "Notes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_StageId",
                table: "Notes",
                column: "StageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Projects_ProjectId",
                table: "Notes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Stages_StageId",
                table: "Notes",
                column: "StageId",
                principalTable: "Stages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Projects_ProjectId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Stages_StageId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_ProjectId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_StageId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ExperimentDeadline",
                table: "ProjectGroups");

            migrationBuilder.DropColumn(
                name: "PlanningDeadline",
                table: "ProjectGroups");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ProjectGroups");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "StageId",
                table: "Notes");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExperimentDeadline",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PlanningDeadline",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
