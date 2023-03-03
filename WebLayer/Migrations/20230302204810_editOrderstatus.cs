using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class editOrderstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Option",
                table: "OrdersStatus");

            migrationBuilder.DropColumn(
                name: "CodeCoupon",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsSeen",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusEnum",
                table: "OrdersStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatusEnum",
                table: "OrdersStatus");

            migrationBuilder.AddColumn<int>(
                name: "Option",
                table: "OrdersStatus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeCoupon",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsSeen",
                table: "Orders",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "83ea514f-ebfe-4098-8a73-b397eb4eeb06", new DateTime(2023, 2, 26, 13, 50, 14, 299, DateTimeKind.Local).AddTicks(986) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "1165758d-b84f-49a0-bea9-5bb6bc5a4a24", new DateTime(2023, 2, 26, 13, 50, 14, 299, DateTimeKind.Local).AddTicks(1048) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "90129346-ee35-4656-a50b-85eb98b37101", new DateTime(2023, 2, 26, 13, 50, 14, 299, DateTimeKind.Local).AddTicks(1054) });
        }
    }
}
