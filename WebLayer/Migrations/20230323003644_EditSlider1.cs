using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLayer.Migrations
{
    public partial class EditSlider1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Sliders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "8b67864d-2cb5-442b-8467-008e4ef1a19a", new DateTime(2023, 3, 22, 14, 36, 43, 10, DateTimeKind.Local).AddTicks(3349) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "40a6c0e7-f0aa-4787-8a2c-ed9b40e7ead6", new DateTime(2023, 3, 22, 14, 36, 43, 10, DateTimeKind.Local).AddTicks(3433) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "28ac891d-729c-47ef-bd92-8d69c7bd69e3", new DateTime(2023, 3, 22, 14, 36, 43, 10, DateTimeKind.Local).AddTicks(3439) });

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_LanguageId",
                table: "Sliders",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sliders_Languages_LanguageId",
                table: "Sliders",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sliders_Languages_LanguageId",
                table: "Sliders");

            migrationBuilder.DropIndex(
                name: "IX_Sliders_LanguageId",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Sliders");

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
    }
}
