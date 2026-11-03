using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EntryLogs_MemberId",
                table: "EntryLogs",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs");

            migrationBuilder.DropIndex(
                name: "IX_EntryLogs_MemberId",
                table: "EntryLogs");
        }
    }
}
