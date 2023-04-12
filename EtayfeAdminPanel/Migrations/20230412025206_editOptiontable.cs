using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editOptiontable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OptionType",
                table: "ProductOptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "e8ae5f09-45aa-42df-a56d-3ce645993ba9", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9252) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "82ec4cfb-f146-40e1-b43a-79b305804337", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9312) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "2689f88f-1b03-4204-b7c2-ed135728b0f0", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9317) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OptionType",
                table: "ProductOptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
