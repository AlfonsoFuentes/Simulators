using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterialEquipmentToMixer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityUnit",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "CapacityValue",
                table: "HCMixers");

            migrationBuilder.AddColumn<string>(
                name: "CapacityUnit",
                table: "MaterialEquipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CapacityValue",
                table: "MaterialEquipments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMixer",
                table: "MaterialEquipments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityUnit",
                table: "MaterialEquipments");

            migrationBuilder.DropColumn(
                name: "CapacityValue",
                table: "MaterialEquipments");

            migrationBuilder.DropColumn(
                name: "IsMixer",
                table: "MaterialEquipments");

            migrationBuilder.AddColumn<string>(
                name: "CapacityUnit",
                table: "HCMixers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CapacityValue",
                table: "HCMixers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
