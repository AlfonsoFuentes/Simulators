using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManySKULine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SKULines_HCSKUs_SKUId",
                table: "SKULines");

            migrationBuilder.DropForeignKey(
                name: "FK_SKULines_Lines_LineId",
                table: "SKULines");

            migrationBuilder.AddForeignKey(
                name: "FK_SKULines_HCSKUs_SKUId",
                table: "SKULines",
                column: "SKUId",
                principalTable: "HCSKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SKULines_Lines_LineId",
                table: "SKULines",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SKULines_HCSKUs_SKUId",
                table: "SKULines");

            migrationBuilder.DropForeignKey(
                name: "FK_SKULines_Lines_LineId",
                table: "SKULines");

            migrationBuilder.AddForeignKey(
                name: "FK_SKULines_HCSKUs_SKUId",
                table: "SKULines",
                column: "SKUId",
                principalTable: "HCSKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SKULines_Lines_LineId",
                table: "SKULines",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
