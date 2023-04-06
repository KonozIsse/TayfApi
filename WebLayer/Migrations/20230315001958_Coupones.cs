using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class Coupones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponName",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CouponNameAr",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Coupons");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "5f2316ac-50db-49bf-a687-4ecdbd4d4215", new DateTime(2023, 3, 14, 14, 19, 56, 577, DateTimeKind.Local).AddTicks(5974) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "d6f5d0fc-e6c9-42d9-ad2b-871a01f58dca", new DateTime(2023, 3, 14, 14, 19, 56, 577, DateTimeKind.Local).AddTicks(6048) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "73a8828e-a72c-42b2-87ad-703de6c9bf82", new DateTime(2023, 3, 14, 14, 19, 56, 577, DateTimeKind.Local).AddTicks(6054) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponName",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponNameAr",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
