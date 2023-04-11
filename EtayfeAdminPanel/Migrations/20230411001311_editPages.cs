using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editPages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "StaticPages");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "StaticPages");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "ShippingMethods");

            migrationBuilder.DropColumn(
                name: "IsFeature",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsViewed",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Addresses");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "4f055df8-ce25-4c9c-b760-6bf4851242a5", new DateTime(2023, 4, 10, 14, 13, 10, 168, DateTimeKind.Local).AddTicks(8556) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "aabbf580-6ea4-4a61-8f32-fc0c1a5f87f2", new DateTime(2023, 4, 10, 14, 13, 10, 168, DateTimeKind.Local).AddTicks(8609) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "a359a881-5c28-47e1-a3ff-35374565ea65", new DateTime(2023, 4, 10, 14, 13, 10, 168, DateTimeKind.Local).AddTicks(8614) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "StaticPages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "StaticPages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsDefault",
                table: "ShippingMethods",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsFeature",
                table: "News",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "IsViewed",
                table: "News",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Contacts",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
    }
}
