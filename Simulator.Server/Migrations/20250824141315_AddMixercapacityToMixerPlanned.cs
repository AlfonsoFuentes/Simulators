using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddMixercapacityToMixerPlanned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MixerCapacityUnit",
                table: "MixerPlanneds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MixerCapacityValue",
                table: "MixerPlanneds",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MixerCapacityUnit",
                table: "MixerPlanneds");

            migrationBuilder.DropColumn(
                name: "MixerCapacityValue",
                table: "MixerPlanneds");
        }
    }
}
