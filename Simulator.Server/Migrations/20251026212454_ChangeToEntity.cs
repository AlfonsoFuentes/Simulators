using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Washouts");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Washouts");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "StreamJoiners");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "StreamJoiners");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "SKULines");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "SKULines");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "SimulationPlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "SimulationPlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Pumps");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Pumps");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PreferedMixer");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PreferedMixer");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PlannedSKUs");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PlannedSKUs");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MixerPlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "MixerPlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MaterialEquipments");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "MaterialEquipments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MainProceses");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "MainProceses");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "LinePlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "LinePlanneds");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "HCSKUs");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "HCSKUs");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "HCMixers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "EquipmentPlannedDownTimes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "EquipmentPlannedDownTimes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ContinuousSystems");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ContinuousSystems");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Conectors");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Conectors");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CompoundProperties");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "CompoundProperties");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CompoundConstants");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "CompoundConstants");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "BackBoneSteps");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "BackBoneSteps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Washouts",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Washouts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Tanks",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Tanks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "StreamJoiners",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "StreamJoiners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "SKULines",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "SKULines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "SimulationPlanneds",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "SimulationPlanneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Pumps",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Pumps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "PreferedMixer",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PreferedMixer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "PlannedSKUs",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PlannedSKUs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Operators",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Operators",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MixerPlanneds",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "MixerPlanneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Materials",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Materials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MaterialEquipments",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "MaterialEquipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MainProceses",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "MainProceses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Lines",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Lines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "LinePlanneds",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "LinePlanneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "HCSKUs",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "HCSKUs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "HCMixers",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "HCMixers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "EquipmentPlannedDownTimes",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "EquipmentPlannedDownTimes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ContinuousSystems",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ContinuousSystems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Conectors",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Conectors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "CompoundProperties",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "CompoundProperties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "CompoundConstants",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "CompoundConstants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "BackBoneSteps",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "BackBoneSteps",
                type: "datetime2",
                nullable: true);
        }
    }
}
