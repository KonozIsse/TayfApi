using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class EditMessageTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "MessageTemplate");

            migrationBuilder.AddColumn<int>(
                name: "NameTemplate",
                table: "MessageTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameTemplate",
                table: "MessageTemplate");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MessageTemplate",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
