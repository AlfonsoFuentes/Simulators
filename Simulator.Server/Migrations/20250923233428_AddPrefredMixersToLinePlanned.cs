using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPrefredMixersToLinePlanned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LinePlannedId",
                table: "HCMixers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HCMixers_LinePlannedId",
                table: "HCMixers",
                column: "LinePlannedId");

            migrationBuilder.AddForeignKey(
                name: "FK_HCMixers_LinePlanneds_LinePlannedId",
                table: "HCMixers",
                column: "LinePlannedId",
                principalTable: "LinePlanneds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HCMixers_LinePlanneds_LinePlannedId",
                table: "HCMixers");

            migrationBuilder.DropIndex(
                name: "IX_HCMixers_LinePlannedId",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "LinePlannedId",
                table: "HCMixers");
        }
    }
}
