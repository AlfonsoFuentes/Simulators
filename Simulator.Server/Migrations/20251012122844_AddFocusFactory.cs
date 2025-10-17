using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusFactory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FocusFactory",
                table: "MainProceses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "FocusFactory",
                table: "MainProceses");
        }
    }
}
