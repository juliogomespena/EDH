using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDH.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SaleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TotalVariableCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalProfit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalAdjustment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    TotalValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "DATETIME('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleItem");

            migrationBuilder.DropTable(
                name: "Sale");
        }
    }
}
