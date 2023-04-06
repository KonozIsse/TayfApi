using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class EditSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LangId",
                table: "Sliders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "aa1523dd-ac45-4244-9e48-9bd5aba1c7a7", new DateTime(2023, 3, 22, 14, 34, 19, 113, DateTimeKind.Local).AddTicks(8568) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "47e71948-0d1b-44e2-b9ef-360a6718ad23", new DateTime(2023, 3, 22, 14, 34, 19, 113, DateTimeKind.Local).AddTicks(8639) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "5e5e4bb5-6405-49ea-8646-047c7d9839b4", new DateTime(2023, 3, 22, 14, 34, 19, 113, DateTimeKind.Local).AddTicks(8648) });

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_LangId",
                table: "Sliders",
                column: "LangId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sliders_Languages_LangId",
                table: "Sliders",
                column: "LangId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sliders_Languages_LangId",
                table: "Sliders");

            migrationBuilder.DropIndex(
                name: "IX_Sliders_LangId",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "LangId",
                table: "Sliders");

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
    }
}
