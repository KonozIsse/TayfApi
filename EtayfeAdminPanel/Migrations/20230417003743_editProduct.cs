using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecialProducts_ProductId",
                table: "SpecialProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales");

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

            migrationBuilder.CreateIndex(
                name: "IX_SpecialProducts_ProductId",
                table: "SpecialProducts",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales",
                column: "ProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecialProducts_ProductId",
                table: "SpecialProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "e8ae5f09-45aa-42df-a56d-3ce645993ba9", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9252) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "82ec4cfb-f146-40e1-b43a-79b305804337", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9312) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "2689f88f-1b03-4204-b7c2-ed135728b0f0", new DateTime(2023, 4, 11, 16, 51, 59, 595, DateTimeKind.Local).AddTicks(9317) });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialProducts_ProductId",
                table: "SpecialProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales",
                column: "ProductId");
        }
    }
}
