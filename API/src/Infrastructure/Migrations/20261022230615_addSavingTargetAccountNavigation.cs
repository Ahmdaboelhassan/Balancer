using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addSavingTargetAccountNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SavingTargetAccount",
                table: "DashboardSettings",
                newName: "SavingTargetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSettings_SavingTargetAccountId",
                table: "DashboardSettings",
                column: "SavingTargetAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_DashboardSettings_Accounts_SavingTargetAccountId",
                table: "DashboardSettings",
                column: "SavingTargetAccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DashboardSettings_Accounts_SavingTargetAccountId",
                table: "DashboardSettings");

            migrationBuilder.DropIndex(
                name: "IX_DashboardSettings_SavingTargetAccountId",
                table: "DashboardSettings");

            migrationBuilder.RenameColumn(
                name: "SavingTargetAccountId",
                table: "DashboardSettings",
                newName: "SavingTargetAccount");
        }
    }
}
