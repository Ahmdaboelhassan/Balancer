using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class journalType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncreasedBalance",
                table: "Journals");

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Journals",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Journals");

            migrationBuilder.AddColumn<bool>(
                name: "IncreasedBalance",
                table: "Journals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
