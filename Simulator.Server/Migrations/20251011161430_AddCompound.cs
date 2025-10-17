using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCompound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompoundConstants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    C1 = table.Column<double>(type: "float", nullable: false),
                    C2 = table.Column<double>(type: "float", nullable: false),
                    C3 = table.Column<double>(type: "float", nullable: false),
                    C4 = table.Column<double>(type: "float", nullable: false),
                    C5 = table.Column<double>(type: "float", nullable: false),
                    C6 = table.Column<double>(type: "float", nullable: false),
                    C7 = table.Column<double>(type: "float", nullable: false),
                    Minimal_Temperature = table.Column<double>(type: "float", nullable: false),
                    Minimal_Temperature_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Maximum_Temperature = table.Column<double>(type: "float", nullable: false),
                    Maximum_Temperature_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CompoundConstants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompoundProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructuralFormula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MolecularWeight = table.Column<double>(type: "float", nullable: false),
                    Critical_Z = table.Column<double>(type: "float", nullable: false),
                    Acentric_Factor = table.Column<double>(type: "float", nullable: false),
                    Acentric_Factor_SRK = table.Column<double>(type: "float", nullable: false),
                    Critical_Temperature = table.Column<double>(type: "float", nullable: false),
                    Critical_Temperature_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Critical_Pressure = table.Column<double>(type: "float", nullable: false),
                    Critical_Pressure_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Critical_Volume = table.Column<double>(type: "float", nullable: false),
                    Critical_Volume_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Boiling_Temperature = table.Column<double>(type: "float", nullable: false),
                    Boiling_Temperature_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Melting_Temperature = table.Column<double>(type: "float", nullable: false),
                    Melting_Temperature_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Asterisk_Volume = table.Column<double>(type: "float", nullable: false),
                    Asterisk_Volume_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VapourPressureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeatOfVaporizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiquidCpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GasCpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiquidViscosityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GasViscosityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiquidThermalConductivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GasThermalConductivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiquidDensityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuperficialTensionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gibbs_Energy_Formation = table.Column<double>(type: "float", nullable: false),
                    Gibbs_Energy_Formation_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enthalpy_Formation = table.Column<double>(type: "float", nullable: false),
                    Enthalpy_Formation_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entropy_Formation = table.Column<double>(type: "float", nullable: false),
                    Entropy_Formation_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_CompoundProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_GasCpId",
                        column: x => x.GasCpId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_GasThermalConductivityId",
                        column: x => x.GasThermalConductivityId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_GasViscosityId",
                        column: x => x.GasViscosityId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_HeatOfVaporizationId",
                        column: x => x.HeatOfVaporizationId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_LiquidCpId",
                        column: x => x.LiquidCpId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_LiquidDensityId",
                        column: x => x.LiquidDensityId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_LiquidThermalConductivityId",
                        column: x => x.LiquidThermalConductivityId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_LiquidViscosityId",
                        column: x => x.LiquidViscosityId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_SuperficialTensionId",
                        column: x => x.SuperficialTensionId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundProperties_CompoundConstants_VapourPressureId",
                        column: x => x.VapourPressureId,
                        principalTable: "CompoundConstants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_GasCpId",
                table: "CompoundProperties",
                column: "GasCpId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_GasThermalConductivityId",
                table: "CompoundProperties",
                column: "GasThermalConductivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_GasViscosityId",
                table: "CompoundProperties",
                column: "GasViscosityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_HeatOfVaporizationId",
                table: "CompoundProperties",
                column: "HeatOfVaporizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_LiquidCpId",
                table: "CompoundProperties",
                column: "LiquidCpId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_LiquidDensityId",
                table: "CompoundProperties",
                column: "LiquidDensityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_LiquidThermalConductivityId",
                table: "CompoundProperties",
                column: "LiquidThermalConductivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_LiquidViscosityId",
                table: "CompoundProperties",
                column: "LiquidViscosityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_SuperficialTensionId",
                table: "CompoundProperties",
                column: "SuperficialTensionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundProperties_VapourPressureId",
                table: "CompoundProperties",
                column: "VapourPressureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompoundProperties");

            migrationBuilder.DropTable(
                name: "CompoundConstants");
        }
    }
}
