using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddEnthalpyCombustion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Enthalpy_Combustion",
                table: "CompoundProperties",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Enthalpy_Combustion_Unit",
                table: "CompoundProperties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enthalpy_Combustion",
                table: "CompoundProperties");

            migrationBuilder.DropColumn(
                name: "Enthalpy_Combustion_Unit",
                table: "CompoundProperties");
        }
    }
}
