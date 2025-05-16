using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorShipmentToUseTrucks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Users_AssignedEmployeeId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_AssignedEmployeeId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "AssignedEmployeeId",
                table: "Shipments");

            migrationBuilder.RenameColumn(
                name: "LoadCapacity",
                table: "Shipments",
                newName: "Direction");

            migrationBuilder.AlterColumn<int>(
                name: "TruckId",
                table: "Shipments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "WeightPerUnit",
                table: "InventoryItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ShipmentInventoryItems",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    InventoryItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentInventoryItems", x => new { x.ShipmentId, x.InventoryItemId });
                    table.ForeignKey(
                        name: "FK_ShipmentInventoryItems_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentInventoryItems_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxCapacityKg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TruckId",
                table: "Shipments",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentInventoryItems_InventoryItemId",
                table: "ShipmentInventoryItems",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Trucks_TruckId",
                table: "Shipments",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Trucks_TruckId",
                table: "Shipments");

            migrationBuilder.DropTable(
                name: "ShipmentInventoryItems");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_TruckId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "WeightPerUnit",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "Direction",
                table: "Shipments",
                newName: "LoadCapacity");

            migrationBuilder.AlterColumn<string>(
                name: "TruckId",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AssignedEmployeeId",
                table: "Shipments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_AssignedEmployeeId",
                table: "Shipments",
                column: "AssignedEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Users_AssignedEmployeeId",
                table: "Shipments",
                column: "AssignedEmployeeId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
