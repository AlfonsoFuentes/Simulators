using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class reorderMixerPlanned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MixerPlanneds_BackBoneSteps_BackBoneStepId",
                table: "MixerPlanneds");

            migrationBuilder.DropForeignKey(
                name: "FK_MixerPlanneds_Materials_BackBoneId",
                table: "MixerPlanneds");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProducingToId",
                table: "MixerPlanneds",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BackBoneId",
                table: "MixerPlanneds",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MixerPlanneds_ProducingToId",
                table: "MixerPlanneds",
                column: "ProducingToId");

            migrationBuilder.AddForeignKey(
                name: "FK_MixerPlanneds_BackBoneSteps_BackBoneStepId",
                table: "MixerPlanneds",
                column: "BackBoneStepId",
                principalTable: "BackBoneSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MixerPlanneds_Materials_BackBoneId",
                table: "MixerPlanneds",
                column: "BackBoneId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MixerPlanneds_Tanks_ProducingToId",
                table: "MixerPlanneds",
                column: "ProducingToId",
                principalTable: "Tanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MixerPlanneds_BackBoneSteps_BackBoneStepId",
                table: "MixerPlanneds");

            migrationBuilder.DropForeignKey(
                name: "FK_MixerPlanneds_Materials_BackBoneId",
                table: "MixerPlanneds");

            migrationBuilder.DropForeignKey(
                name: "FK_MixerPlanneds_Tanks_ProducingToId",
                table: "MixerPlanneds");

            migrationBuilder.DropIndex(
                name: "IX_MixerPlanneds_ProducingToId",
                table: "MixerPlanneds");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProducingToId",
                table: "MixerPlanneds",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BackBoneId",
                table: "MixerPlanneds",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MixerPlanneds_BackBoneSteps_BackBoneStepId",
                table: "MixerPlanneds",
                column: "BackBoneStepId",
                principalTable: "BackBoneSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MixerPlanneds_Materials_BackBoneId",
                table: "MixerPlanneds",
                column: "BackBoneId",
                principalTable: "Materials",
                principalColumn: "Id");
        }
    }
}
