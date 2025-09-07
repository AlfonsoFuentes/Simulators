using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddEA_case : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineSKU");

            migrationBuilder.DropColumn(
                name: "Case_Shift",
                table: "PlannedSKUs");

            migrationBuilder.AddColumn<double>(
                name: "Case_Shift",
                table: "SKULines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Case_Shift",
                table: "SKULines");

            migrationBuilder.AddColumn<double>(
                name: "Case_Shift",
                table: "PlannedSKUs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "LineSKU",
                columns: table => new
                {
                    LinesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineSKU", x => new { x.LinesId, x.SKUsId });
                    table.ForeignKey(
                        name: "FK_LineSKU_HCSKUs_SKUsId",
                        column: x => x.SKUsId,
                        principalTable: "HCSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineSKU_Lines_LinesId",
                        column: x => x.LinesId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineSKU_SKUsId",
                table: "LineSKU",
                column: "SKUsId");
        }
    }
}
