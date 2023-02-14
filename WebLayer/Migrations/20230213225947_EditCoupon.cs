using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class EditCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Products",
                table: "Coupons");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "bcc7ab5c-ac97-47e1-ad4a-34c21fad673c", new DateTime(2023, 2, 13, 12, 59, 46, 444, DateTimeKind.Local).AddTicks(2998) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "59ca691e-2755-4a6e-a1a3-eebb6d4d158f", new DateTime(2023, 2, 13, 12, 59, 46, 444, DateTimeKind.Local).AddTicks(3062) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "36a7143d-bc07-4324-a09e-1f9dbb13ede3", new DateTime(2023, 2, 13, 12, 59, 46, 444, DateTimeKind.Local).AddTicks(3067) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                table: "Coupons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Products",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "76c8eb47-b6fc-42bc-bace-2b1de55080e6", new DateTime(2023, 2, 13, 10, 54, 35, 936, DateTimeKind.Local).AddTicks(108) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "87d00ca5-c3bc-4a7b-ab6c-f146c380c1db", new DateTime(2023, 2, 13, 10, 54, 35, 936, DateTimeKind.Local).AddTicks(157) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "965f1c82-f876-40d6-b7fc-3e2b56cdb1dc", new DateTime(2023, 2, 13, 10, 54, 35, 936, DateTimeKind.Local).AddTicks(161) });
        }
    }
}
