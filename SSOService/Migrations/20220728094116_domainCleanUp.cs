using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSOService.Migrations
{
    public partial class domainCleanUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "SubscriptionServices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubscriptionServices");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "SubscriptionServices");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "SubscriptionServices");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "RoleClaims");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Subscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Applications");

            migrationBuilder.AddColumn<Guid>(
                name: "ConcurrencyStamp",
                table: "UserPermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserPermissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "UserPermissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "UserPermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConcurrencyStamp",
                table: "SubscriptionServices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SubscriptionServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "SubscriptionServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "SubscriptionServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConcurrencyStamp",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "RolePermissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RolePermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConcurrencyStamp",
                table: "RoleClaims",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RoleClaims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "RoleClaims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "RoleClaims",
                type: "datetime2",
                nullable: true);
        }
    }
}
