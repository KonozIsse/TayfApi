using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class AddenumtoLink1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NavSubmenu",
                table: "Links",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "acd2a84e-8f30-4b8c-84d1-bd5d0cd5093a", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6012) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "152baac4-ac96-4486-8923-9d4e3676f70c", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "3acfde42-8ab1-4d06-8817-ed47916359cf", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6079) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NavSubmenu",
                table: "Links");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "2b4e8375-3532-45d3-b794-27782dac1e20", new DateTime(2023, 4, 18, 11, 35, 29, 524, DateTimeKind.Local).AddTicks(7039) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "1f1ba777-3566-4479-953a-2a44703605ac", new DateTime(2023, 4, 18, 11, 35, 29, 524, DateTimeKind.Local).AddTicks(7158) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "b63b829f-c682-401b-819b-2a9f8a02242b", new DateTime(2023, 4, 18, 11, 35, 29, 524, DateTimeKind.Local).AddTicks(7163) });
        }
    }
}
