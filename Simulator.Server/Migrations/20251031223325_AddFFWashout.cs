using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFFWashout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Washouts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Washouts");
        }
    }
}
