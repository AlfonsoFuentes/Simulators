using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPtocessquipmentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "Tanks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "Pumps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "Operators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "Lines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "HCMixers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProccesEquipmentType",
                table: "ContinuousSystems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "Pumps");

            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "ProccesEquipmentType",
                table: "ContinuousSystems");
        }
    }
}
