using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addnewCostCentersRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetailCostCenter_CostCenters_CostCenterId",
                table: "JournalDetailCostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetailCostCenter_JournalDetails_JournalDetailId",
                table: "JournalDetailCostCenter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalDetailCostCenter",
                table: "JournalDetailCostCenter");

            migrationBuilder.RenameTable(
                name: "JournalDetailCostCenter",
                newName: "JournalDetailCostCenters");

            migrationBuilder.RenameIndex(
                name: "IX_JournalDetailCostCenter_JournalDetailId",
                table: "JournalDetailCostCenters",
                newName: "IX_JournalDetailCostCenters_JournalDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_JournalDetailCostCenter_CostCenterId",
                table: "JournalDetailCostCenters",
                newName: "IX_JournalDetailCostCenters_CostCenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalDetailCostCenters",
                table: "JournalDetailCostCenters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetailCostCenters_CostCenters_CostCenterId",
                table: "JournalDetailCostCenters",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetailCostCenters_JournalDetails_JournalDetailId",
                table: "JournalDetailCostCenters",
                column: "JournalDetailId",
                principalTable: "JournalDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetailCostCenters_CostCenters_CostCenterId",
                table: "JournalDetailCostCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetailCostCenters_JournalDetails_JournalDetailId",
                table: "JournalDetailCostCenters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalDetailCostCenters",
                table: "JournalDetailCostCenters");

            migrationBuilder.RenameTable(
                name: "JournalDetailCostCenters",
                newName: "JournalDetailCostCenter");

            migrationBuilder.RenameIndex(
                name: "IX_JournalDetailCostCenters_JournalDetailId",
                table: "JournalDetailCostCenter",
                newName: "IX_JournalDetailCostCenter_JournalDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_JournalDetailCostCenters_CostCenterId",
                table: "JournalDetailCostCenter",
                newName: "IX_JournalDetailCostCenter_CostCenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalDetailCostCenter",
                table: "JournalDetailCostCenter",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetailCostCenter_CostCenters_CostCenterId",
                table: "JournalDetailCostCenter",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetailCostCenter_JournalDetails_JournalDetailId",
                table: "JournalDetailCostCenter",
                column: "JournalDetailId",
                principalTable: "JournalDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
