using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusFactory3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "HCSKUs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "HCSKUs");
        }
    }
}
