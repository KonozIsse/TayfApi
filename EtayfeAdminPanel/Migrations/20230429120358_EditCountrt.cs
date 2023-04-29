using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtayfeAdminPanel.Migrations
{
    public partial class EditCountrt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Images_ImgId",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Images_ImgId",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "ImgId",
                table: "Countries",
                newName: "ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_ImgId",
                table: "Countries",
                newName: "IX_Countries_ImageId");

            migrationBuilder.AlterColumn<int>(
                name: "ImgId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "39a843c7-4f8e-433b-b5ab-c5a4293ebd53", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4012) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "fcd59888-ceac-4e5c-9f20-12c572841b83", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4102) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "60b78f82-174e-4bdf-89e5-11608cf617bd", new DateTime(2023, 4, 29, 2, 3, 57, 569, DateTimeKind.Local).AddTicks(4108) });

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Images_ImageId",
                table: "Countries",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Images_ImgId",
                table: "News",
                column: "ImgId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Images_ImageId",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Images_ImgId",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Countries",
                newName: "ImgId");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_ImageId",
                table: "Countries",
                newName: "IX_Countries_ImgId");

            migrationBuilder.AlterColumn<int>(
                name: "ImgId",
                table: "News",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "acd2a84e-8f30-4b8c-84d1-bd5d0cd5093a", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6012) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "152baac4-ac96-4486-8923-9d4e3676f70c", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt" },
                values: new object[] { "3acfde42-8ab1-4d06-8817-ed47916359cf", new DateTime(2023, 4, 18, 11, 37, 33, 566, DateTimeKind.Local).AddTicks(6079) });

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Images_ImgId",
                table: "Countries",
                column: "ImgId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Images_ImgId",
                table: "News",
                column: "ImgId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}
