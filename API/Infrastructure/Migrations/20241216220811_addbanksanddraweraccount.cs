using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addbanksanddraweraccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevenueAccountNumber",
                table: "Settings",
                newName: "RevenueAccount");

            migrationBuilder.RenameColumn(
                name: "LiabilitiesAccountNumber",
                table: "Settings",
                newName: "LiabilitiesAccount");

            migrationBuilder.RenameColumn(
                name: "ExpensesAccountNumber",
                table: "Settings",
                newName: "ExpensesAccount");

            migrationBuilder.RenameColumn(
                name: "AssetsAccountNumber",
                table: "Settings",
                newName: "DrawersAccount");

            migrationBuilder.AddColumn<int>(
                name: "AssetsAccount",
                table: "Settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BanksAccount",
                table: "Settings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetsAccount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "BanksAccount",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "RevenueAccount",
                table: "Settings",
                newName: "RevenueAccountNumber");

            migrationBuilder.RenameColumn(
                name: "LiabilitiesAccount",
                table: "Settings",
                newName: "LiabilitiesAccountNumber");

            migrationBuilder.RenameColumn(
                name: "ExpensesAccount",
                table: "Settings",
                newName: "ExpensesAccountNumber");

            migrationBuilder.RenameColumn(
                name: "DrawersAccount",
                table: "Settings",
                newName: "AssetsAccountNumber");
        }
    }
}
