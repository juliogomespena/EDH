using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDH.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSaleAndSaleLinesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItem");

            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "SaleLine",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaleLine_SaleId",
                table: "SaleLine",
                column: "SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLine_Sale_SaleId",
                table: "SaleLine",
                column: "SaleId",
                principalTable: "Sale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleLine_Sale_SaleId",
                table: "SaleLine");

            migrationBuilder.DropIndex(
                name: "IX_SaleLine_SaleId",
                table: "SaleLine");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "SaleLine");

            migrationBuilder.CreateTable(
                name: "SaleItem",
                columns: table => new
                {
                    ItemsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItem", x => new { x.ItemsId, x.SalesId });
                    table.ForeignKey(
                        name: "FK_SaleItem_Item_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItem_Sale_SalesId",
                        column: x => x.SalesId,
                        principalTable: "Sale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_SalesId",
                table: "SaleItem",
                column: "SalesId");
        }
    }
}
