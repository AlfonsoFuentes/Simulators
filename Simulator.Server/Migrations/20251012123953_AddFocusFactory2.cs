using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusFactory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Tanks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Pumps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Operators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Lines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "HCMixers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "ContinuousSystems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Pumps");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "ContinuousSystems");
        }
    }
}
