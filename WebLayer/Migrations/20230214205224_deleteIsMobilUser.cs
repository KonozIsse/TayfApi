using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class deleteIsMobilUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMobileVerified",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "478af2b8-7cbb-4d67-81ca-ede96cf888d5", new DateTime(2023, 2, 14, 10, 52, 22, 697, DateTimeKind.Local).AddTicks(3959) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "41e98959-f040-447a-b8d2-d4e2e98a2951", new DateTime(2023, 2, 14, 10, 52, 22, 697, DateTimeKind.Local).AddTicks(4036) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "95fe980c-33cc-4e03-a697-c1b2e594407e", new DateTime(2023, 2, 14, 10, 52, 22, 697, DateTimeKind.Local).AddTicks(4041) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMobileVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "d23cee9a-f861-41aa-b331-ada0bc857897", new DateTime(2023, 2, 13, 14, 49, 22, 567, DateTimeKind.Local).AddTicks(857) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "1a8ba188-b3b8-408e-a89b-c6c565107a96", new DateTime(2023, 2, 13, 14, 49, 22, 567, DateTimeKind.Local).AddTicks(929) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "0281109d-0c14-4a7a-a929-2eb7a08bc5bf", new DateTime(2023, 2, 13, 14, 49, 22, 567, DateTimeKind.Local).AddTicks(934) });
        }
    }
}
