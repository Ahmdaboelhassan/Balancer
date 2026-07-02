using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addnewCostCentersIsDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "JournalDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JournalDetailCostCenters",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JournalDetailCostCenters");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "JournalDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
