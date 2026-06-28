using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addCostCenter2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualCreatedAt",
                table: "Journals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondCostCenterId",
                table: "JournalDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_SecondCostCenterId",
                table: "JournalDetails",
                column: "SecondCostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_CostCenters_SecondCostCenterId",
                table: "JournalDetails",
                column: "SecondCostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_CostCenters_SecondCostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropIndex(
                name: "IX_JournalDetails_SecondCostCenterId",
                table: "JournalDetails");

            migrationBuilder.DropColumn(
                name: "ActualCreatedAt",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "SecondCostCenterId",
                table: "JournalDetails");
        }
    }
}
