using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class editPaymentstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Environment",
                table: "PaymentMethods");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "PaymentMethods",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "PaymentMethods");

            migrationBuilder.AddColumn<short>(
                name: "Environment",
                table: "PaymentMethods",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "030ce7d1-e058-484b-a832-5c975d73efe5", new DateTime(2023, 3, 2, 10, 48, 9, 623, DateTimeKind.Local).AddTicks(4576) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "84c090b1-aaa4-40c6-9aaa-5d7feab019f6", new DateTime(2023, 3, 2, 10, 48, 9, 623, DateTimeKind.Local).AddTicks(4642) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "e87ccd01-8356-438b-8914-7ef8078c1f43", new DateTime(2023, 3, 2, 10, 48, 9, 623, DateTimeKind.Local).AddTicks(4648) });
        }
    }
}
