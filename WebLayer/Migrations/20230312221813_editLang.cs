using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class editLang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Languages");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "acde01be-6b04-4e6b-ac6b-e50e0d72dce2", new DateTime(2023, 3, 12, 12, 18, 11, 94, DateTimeKind.Local).AddTicks(7692) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "db00a680-a091-4a94-9559-41a2ba9c3afa", new DateTime(2023, 3, 12, 12, 18, 11, 94, DateTimeKind.Local).AddTicks(7770) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "be7a6ca2-9d41-4bb9-9206-950c444c2f33", new DateTime(2023, 3, 12, 12, 18, 11, 94, DateTimeKind.Local).AddTicks(7776) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsDefault",
                table: "Languages",
                type: "smallint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "bd25df95-4451-473c-afad-f34872e4ac0b", new DateTime(2023, 3, 2, 12, 31, 7, 557, DateTimeKind.Local).AddTicks(2239) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "dc4d94a7-54e7-4eeb-8da6-f549ae2c22fb", new DateTime(2023, 3, 2, 12, 31, 7, 557, DateTimeKind.Local).AddTicks(2296) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "d2dedc6a-10ce-4813-aa4b-1882f1c58c44", new DateTime(2023, 3, 2, 12, 31, 7, 557, DateTimeKind.Local).AddTicks(2300) });
        }
    }
}
