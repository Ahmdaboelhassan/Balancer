using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addBudgetAccountTypeAndPaidMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BestBudget",
                table: "BudgetAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumBudget",
                table: "BudgetAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaidMethodId",
                table: "BudgetAccounts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "BudgetAccounts",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestBudget",
                table: "BudgetAccounts");

            migrationBuilder.DropColumn(
                name: "MaximumBudget",
                table: "BudgetAccounts");

            migrationBuilder.DropColumn(
                name: "PaidMethodId",
                table: "BudgetAccounts");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "BudgetAccounts");
        }
    }
}
