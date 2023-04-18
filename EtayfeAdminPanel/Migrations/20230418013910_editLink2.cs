using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editLink2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Show",
                table: "Links");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Show",
                table: "Links",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "93c8562a-f0d2-4995-abef-13ece73ffbf6", new DateTime(2023, 4, 17, 15, 36, 20, 37, DateTimeKind.Local).AddTicks(7038) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "f893953e-5bc9-46b2-b4f6-b1da476936d0", new DateTime(2023, 4, 17, 15, 36, 20, 37, DateTimeKind.Local).AddTicks(7094) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "d828a36d-d466-471c-8131-f7bd42259b8b", new DateTime(2023, 4, 17, 15, 36, 20, 37, DateTimeKind.Local).AddTicks(7099) });
        }
    }
}
