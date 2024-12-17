using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultPeriodDays",
                table: "Settings",
                newName: "RevenueAccount");

            migrationBuilder.RenameColumn(
                name: "DefaultDepitAccount",
                table: "Settings",
                newName: "LiabilitiesAccount");

            migrationBuilder.AddColumn<int>(
                name: "AssetsAccount",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultDebitAccount",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpensesAccount",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelFourDigits",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelOneDigits",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelThreeDigits",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelTwoDigits",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetsAccount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DefaultDebitAccount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ExpensesAccount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LevelFourDigits",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LevelOneDigits",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LevelThreeDigits",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LevelTwoDigits",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "RevenueAccount",
                table: "Settings",
                newName: "DefaultPeriodDays");

            migrationBuilder.RenameColumn(
                name: "LiabilitiesAccount",
                table: "Settings",
                newName: "DefaultDepitAccount");
        }
    }
}
