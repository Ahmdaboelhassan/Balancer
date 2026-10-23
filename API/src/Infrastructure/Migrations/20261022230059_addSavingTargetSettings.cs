using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addSavingTargetSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SavingTargetAccount",
                table: "DashboardSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavingTargetColor",
                table: "DashboardSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavingTargetDisplayName",
                table: "DashboardSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavingTargetAccount",
                table: "DashboardSettings");

            migrationBuilder.DropColumn(
                name: "SavingTargetColor",
                table: "DashboardSettings");

            migrationBuilder.DropColumn(
                name: "SavingTargetDisplayName",
                table: "DashboardSettings");
        }
    }
}
