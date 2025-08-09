using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDH.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeStockAlertThresholdConfigurationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AlertThreshold",
                table: "InventoryItem",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AlertThreshold",
                table: "InventoryItem",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
