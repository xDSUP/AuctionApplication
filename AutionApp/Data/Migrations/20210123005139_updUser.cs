using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutionApp.Data.Migrations
{
    public partial class updUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Lots",
                type: "image",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "image");

            migrationBuilder.CreateIndex(
                name: "IX_Sells_LotId",
                table: "Sells",
                column: "LotId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sells_LotId",
                table: "Sells");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Lots",
                type: "image",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "image",
                oldNullable: true);
        }
    }
}
