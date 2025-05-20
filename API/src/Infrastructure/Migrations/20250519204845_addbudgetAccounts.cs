using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addbudgetAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DashbourdAccounts",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "CurrentCashAccount",
                table: "DashbourdAccounts");

            migrationBuilder.DropColumn(
                name: "DayRate",
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

            migrationBuilder.RenameTable(
                name: "DashbourdAccounts",
                newName: "DashboardSettings");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultDayRate",
                table: "Settings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DashboardSettings",
                table: "DashboardSettings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BudgetAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAccounts_AccountId",
                table: "BudgetAccounts",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DashboardSettings",
                table: "DashboardSettings");

            migrationBuilder.DropColumn(
                name: "DefaultDayRate",
                table: "Settings");

            migrationBuilder.RenameTable(
                name: "DashboardSettings",
                newName: "DashbourdAccounts");

            migrationBuilder.AddColumn<int>(
                name: "CurrentCashAccount",
                table: "DashbourdAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DayRate",
                table: "DashbourdAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_DashbourdAccounts",
                table: "DashbourdAccounts",
                column: "Id");
        }
    }
}
