using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordCode",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SocialImage",
                table: "AspNetUsers",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "SocialId",
                table: "AspNetUsers",
                newName: "FcmToken");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "7634a8b4-0b10-411a-a238-1be3284a56e8", new DateTime(2023, 4, 9, 15, 4, 7, 615, DateTimeKind.Local).AddTicks(3009) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "13e77ea3-59a9-4e36-a5f8-dd8d7695acd8", new DateTime(2023, 4, 9, 15, 4, 7, 615, DateTimeKind.Local).AddTicks(3074) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "b74f0891-67fa-4eb8-a668-bf10b4bfef6c", new DateTime(2023, 4, 9, 15, 4, 7, 615, DateTimeKind.Local).AddTicks(3081) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "AspNetUsers",
                newName: "SocialImage");

            migrationBuilder.RenameColumn(
                name: "FcmToken",
                table: "AspNetUsers",
                newName: "SocialId");

            migrationBuilder.AddColumn<int>(
                name: "ResetPasswordCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "cdfd40f9-1cf5-47c0-85d3-150c59874957", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5532) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "af3c5323-3caa-4b8d-b4c1-7eda3214fa88", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5585) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "185ffcdc-4663-4083-9fbc-5c7eaa8303a6", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5590) });
        }
    }
}
