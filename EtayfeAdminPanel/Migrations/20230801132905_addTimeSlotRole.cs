using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class addTimeSlotRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "UpdatedAt",
                table: "AspNetRoles",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CreatedAt",
                table: "AspNetRoles",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "IsVendorLink" },
                values: new object[] { "9a5f1c05-09be-4853-b250-aead24faf414", false });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "IsVendorLink" },
                values: new object[] { "25eb129d-89cf-4030-9491-87aa0c641635", false });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "IsVendorLink" },
                values: new object[] { "e6c2e49b-6b77-421c-9125-12965393d411", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "IsVendorLink" },
                values: new object[] { "39a843c7-4f8e-433b-b5ab-c5a4293ebd53", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4012), null });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "IsVendorLink" },
                values: new object[] { "fcd59888-ceac-4e5c-9f20-12c572841b83", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4102), null });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "IsVendorLink" },
                values: new object[] { "60b78f82-174e-4bdf-89e5-11608cf617bd", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4108), null });
        }
    }
}
