using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class add_deleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_Accounts_JournalId",
                table: "JournalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_Journals_JournalId1",
                table: "JournalDetails");

            migrationBuilder.DropIndex(
                name: "IX_JournalDetails_JournalId1",
                table: "JournalDetails");

            migrationBuilder.DropColumn(
                name: "JournalId1",
                table: "JournalDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_Journals_JournalId",
                table: "JournalDetails",
                column: "JournalId",
                principalTable: "Journals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetails_Journals_JournalId",
                table: "JournalDetails");

            migrationBuilder.AddColumn<int>(
                name: "JournalId1",
                table: "JournalDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_JournalId1",
                table: "JournalDetails",
                column: "JournalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_Accounts_JournalId",
                table: "JournalDetails",
                column: "JournalId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetails_Journals_JournalId1",
                table: "JournalDetails",
                column: "JournalId1",
                principalTable: "Journals",
                principalColumn: "Id");
        }
    }
}
