using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI4Green4Students.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldResponseValueProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.Sql("ALTER TABLE \"FieldResponseValues\" ALTER COLUMN \"Value\" TYPE jsonb USING \"Value\"::jsonb;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "FieldResponseValues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
