using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSOService.Migrations
{
    public partial class mainDomainSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueName",
                table: "Entities",
                newName: "Code");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Entities",
                newName: "UniqueName");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
