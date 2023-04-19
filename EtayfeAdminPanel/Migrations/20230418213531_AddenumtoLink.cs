using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class AddenumtoLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "8261abff-bb9e-40f5-b2ee-d3249b8732e0", new DateTime(2023, 4, 17, 15, 39, 9, 426, DateTimeKind.Local).AddTicks(3080) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "9ca78667-49fa-4bb4-b03c-65889c3845eb", new DateTime(2023, 4, 17, 15, 39, 9, 426, DateTimeKind.Local).AddTicks(3140) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "abb15bfe-6dc5-4633-91ff-c3612a9b8a20", new DateTime(2023, 4, 17, 15, 39, 9, 426, DateTimeKind.Local).AddTicks(3145) });
        }
    }
}
