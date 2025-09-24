using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPrefredMixersToLinePlanned2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreferedMixer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MixerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinePlannedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferedMixer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferedMixer_HCMixers_MixerId",
                        column: x => x.MixerId,
                        principalTable: "HCMixers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreferedMixer_LinePlanneds_LinePlannedId",
                        column: x => x.LinePlannedId,
                        principalTable: "LinePlanneds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreferedMixer_LinePlannedId",
                table: "PreferedMixer",
                column: "LinePlannedId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferedMixer_MixerId",
                table: "PreferedMixer",
                column: "MixerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreferedMixer");
        }
    }
}
