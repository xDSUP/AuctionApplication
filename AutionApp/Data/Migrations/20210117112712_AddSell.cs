using Microsoft.EntityFrameworkCore.Migrations;

namespace AutionApp.Data.Migrations
{
    public partial class AddSell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Categories_CategoryId",
                table: "Lots");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Lots",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Sells",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LotId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sells", x => new { x.LotId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Sells_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sells_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sells_UserId",
                table: "Sells",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Categories_CategoryId",
                table: "Lots",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Categories_CategoryId",
                table: "Lots");

            migrationBuilder.DropTable(
                name: "Sells");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Lots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Categories_CategoryId",
                table: "Lots",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
