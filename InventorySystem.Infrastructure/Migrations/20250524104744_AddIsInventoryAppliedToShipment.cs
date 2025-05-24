using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInventoryAppliedToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInventoryApplied",
                table: "Shipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InventoryItemId1",
                table: "ShipmentInventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentInventoryItems_InventoryItemId1",
                table: "ShipmentInventoryItems",
                column: "InventoryItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentInventoryItems_InventoryItems_InventoryItemId1",
                table: "ShipmentInventoryItems",
                column: "InventoryItemId1",
                principalTable: "InventoryItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentInventoryItems_InventoryItems_InventoryItemId1",
                table: "ShipmentInventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentInventoryItems_InventoryItemId1",
                table: "ShipmentInventoryItems");

            migrationBuilder.DropColumn(
                name: "IsInventoryApplied",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "InventoryItemId1",
                table: "ShipmentInventoryItems");
        }
    }
}
