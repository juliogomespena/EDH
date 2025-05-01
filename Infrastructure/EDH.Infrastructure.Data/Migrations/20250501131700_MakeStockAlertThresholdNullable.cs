using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDH.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeStockAlertThresholdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AlertThreshold",
                table: "InventoryItem",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AlertThreshold",
                table: "InventoryItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldDefaultValue: 0);
        }
    }
}
