using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddrestrictionOperators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaxRestrictionTimeUnit",
                table: "SimulationPlanneds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MaxRestrictionTimeValue",
                table: "SimulationPlanneds",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "OperatorHasNotRestrictionToInitBatch",
                table: "SimulationPlanneds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRestrictionTimeUnit",
                table: "SimulationPlanneds");

            migrationBuilder.DropColumn(
                name: "MaxRestrictionTimeValue",
                table: "SimulationPlanneds");

            migrationBuilder.DropColumn(
                name: "OperatorHasNotRestrictionToInitBatch",
                table: "SimulationPlanneds");
        }
    }
}
