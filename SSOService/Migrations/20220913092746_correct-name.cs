using Microsoft.EntityFrameworkCore.Migrations;

namespace SSOService.Migrations
{
    public partial class correctname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationAuthentifications",
                table: "ApplicationAuthentifications");

            migrationBuilder.RenameTable(
                name: "ApplicationAuthentifications",
                newName: "ApplicationAuthentications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationAuthentications",
                table: "ApplicationAuthentications",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationAuthentications",
                table: "ApplicationAuthentications");

            migrationBuilder.RenameTable(
                name: "ApplicationAuthentications",
                newName: "ApplicationAuthentifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationAuthentifications",
                table: "ApplicationAuthentifications",
                column: "Id");
        }
    }
}
