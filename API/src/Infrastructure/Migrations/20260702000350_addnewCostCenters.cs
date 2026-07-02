using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addnewCostCenters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JournalDetailCostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalDetailId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalDetailCostCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalDetailCostCenter_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalDetailCostCenter_JournalDetails_JournalDetailId",
                        column: x => x.JournalDetailId,
                        principalTable: "JournalDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetailCostCenter_CostCenterId",
                table: "JournalDetailCostCenter",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetailCostCenter_JournalDetailId",
                table: "JournalDetailCostCenter",
                column: "JournalDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalDetailCostCenter");
        }
    }
}
