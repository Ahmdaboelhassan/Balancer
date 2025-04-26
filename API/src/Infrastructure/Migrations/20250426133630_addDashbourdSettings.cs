using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addDashbourdSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentCashAccount",
                table: "DashbourdAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GamieaLiabilitiesAccount",
                table: "DashbourdAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "GamieaLiabilitiesTarget",
                table: "DashbourdAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlySavingsTarget",
                table: "DashbourdAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherExpensesTarget",
                table: "DashbourdAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PeriodExpensesTarget",
                table: "DashbourdAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PeriodsExpensesAccount",
                table: "DashbourdAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCashAccount",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "GamieaLiabilitiesAccount",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "GamieaLiabilitiesTarget",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "MonthlySavingsTarget",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "OtherExpensesTarget",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "PeriodExpensesTarget",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "PeriodsExpensesAccount",
                table: "DashbourdAccounts");
        }
    }
}
