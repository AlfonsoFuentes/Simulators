using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamJoiner2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "StreamJoiners");

            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "IsForWashing",
                table: "ContinuousSystems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "Tanks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "StreamJoiners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "Operators",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "Lines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "HCMixers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForWashing",
                table: "ContinuousSystems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
