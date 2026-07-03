using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class makelastupdatedatnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_CostCenters_CostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_CostCenters_SecondCostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropIndex(
                name: "IX_JournalDetails_CostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropIndex(
                name: "IX_JournalDetails_SecondCostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropColumn(
                name: "SecondCostCenterId",
                table: "JournalDetails");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Evaluations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Journals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "JournalDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondCostCenterId",
                table: "JournalDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Evaluations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_CostCenterId",
                table: "JournalDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_SecondCostCenterId",
                table: "JournalDetails",
                column: "SecondCostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_CostCenters_CostCenterId",
                table: "JournalDetails",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_CostCenters_SecondCostCenterId",
                table: "JournalDetails",
                column: "SecondCostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id");
        }
    }
}
