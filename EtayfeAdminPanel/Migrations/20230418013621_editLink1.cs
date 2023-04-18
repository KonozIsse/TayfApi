using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editLink1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "Links");

            migrationBuilder.RenameColumn(
                name: "TitleLinkAr",
                table: "Links",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "TitleLink",
                table: "Links",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ParentLinkId",
                table: "Links",
                newName: "ParentId");

            migrationBuilder.RenameColumn(
                name: "OrderedId",
                table: "Links",
                newName: "OrderId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Links",
                newName: "TitleLinkAr");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Links",
                newName: "TitleLink");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Links",
                newName: "ParentLinkId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Links",
                newName: "OrderedId");

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "Links",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
