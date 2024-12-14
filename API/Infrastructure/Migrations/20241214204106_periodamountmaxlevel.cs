using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class periodamountmaxlevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevenueAccount",
                table: "Settings",
                newName: "RevenueAccountNumber");

            migrationBuilder.RenameColumn(
                name: "LiabilitiesAccount",
                table: "Settings",
                newName: "MaxAccountLevel");

            migrationBuilder.RenameColumn(
                name: "ExpensesAccount",
                table: "Settings",
                newName: "LiabilitiesAccountNumber");

            migrationBuilder.RenameColumn(
                name: "AssetsAccount",
                table: "Settings",
                newName: "ExpensesAccountNumber");

            migrationBuilder.AddColumn<int>(
                name: "AssetsAccountNumber",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Journals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetsAccountNumber",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Journals");

            migrationBuilder.RenameColumn(
                name: "RevenueAccountNumber",
                table: "Settings",
                newName: "RevenueAccount");

            migrationBuilder.RenameColumn(
                name: "MaxAccountLevel",
                table: "Settings",
                newName: "LiabilitiesAccount");

            migrationBuilder.RenameColumn(
                name: "LiabilitiesAccountNumber",
                table: "Settings",
                newName: "ExpensesAccount");

            migrationBuilder.RenameColumn(
                name: "ExpensesAccountNumber",
                table: "Settings",
                newName: "AssetsAccount");
        }
    }
}
