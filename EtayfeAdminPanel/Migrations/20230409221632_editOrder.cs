using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class editOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodsId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingMethods_ShippingMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodsId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingMethodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HashedCtpAndPayment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodsId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingMethodId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Orders",
                newName: "Transaction");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "cdfd40f9-1cf5-47c0-85d3-150c59874957", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5532) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "af3c5323-3caa-4b8d-b4c1-7eda3214fa88", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5585) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "185ffcdc-4663-4083-9fbc-5c7eaa8303a6", new DateTime(2023, 4, 9, 12, 16, 30, 608, DateTimeKind.Local).AddTicks(5590) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Transaction",
                table: "Orders",
                newName: "TransactionId");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HashedCtpAndPayment",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodsId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingMethodId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "d316d1b5-aca7-4143-964b-989a9b2ee91c", new DateTime(2023, 4, 4, 22, 4, 11, 754, DateTimeKind.Local).AddTicks(641) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "61d368d7-5ff0-416c-ba27-2b75b485763a", new DateTime(2023, 4, 4, 22, 4, 11, 754, DateTimeKind.Local).AddTicks(709) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "76ca5f49-04a5-4c58-9939-07f13bb7cb18", new DateTime(2023, 4, 4, 22, 4, 11, 754, DateTimeKind.Local).AddTicks(769) });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodsId",
                table: "Orders",
                column: "PaymentMethodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingMethodId",
                table: "Orders",
                column: "ShippingMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                table: "Orders",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodsId",
                table: "Orders",
                column: "PaymentMethodsId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingMethods_ShippingMethodId",
                table: "Orders",
                column: "ShippingMethodId",
                principalTable: "ShippingMethods",
                principalColumn: "Id");
        }
    }
}
