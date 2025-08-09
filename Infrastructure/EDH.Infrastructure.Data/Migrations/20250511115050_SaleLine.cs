using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDH.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SaleLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitVariableCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalVariableCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Adjustment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Profit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleLine_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleLine_ItemId",
                table: "SaleLine",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleLine");
        }
    }
}
