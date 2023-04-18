using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "4ead1c86-c631-4113-a319-d3abf09a710e", new DateTime(2023, 4, 17, 15, 32, 21, 879, DateTimeKind.Local).AddTicks(6539) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "2734b30e-a4c9-4352-b00a-58701c68f32a", new DateTime(2023, 4, 17, 15, 32, 21, 879, DateTimeKind.Local).AddTicks(6603) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "81c55a10-87a2-468e-937c-aebcf053a61a", new DateTime(2023, 4, 17, 15, 32, 21, 879, DateTimeKind.Local).AddTicks(6610) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "ed91762e-e3fc-45d4-b58d-1f633145d678", new DateTime(2023, 4, 16, 14, 37, 42, 586, DateTimeKind.Local).AddTicks(2819) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "0c795ba4-763b-4d0f-9396-d3f097062f9b", new DateTime(2023, 4, 16, 14, 37, 42, 586, DateTimeKind.Local).AddTicks(2874) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "ae5e3892-d98e-4acf-a483-64921d8752bd", new DateTime(2023, 4, 16, 14, 37, 42, 586, DateTimeKind.Local).AddTicks(2878) });
        }
    }
}
