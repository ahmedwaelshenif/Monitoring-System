using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIdentityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Visitors",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Registration_Requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DashboardAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_Email",
                table: "Visitors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_MemberId",
                table: "Visitors",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registration_Requests_Email",
                table: "Registration_Requests",
                column: "Email",
                unique: true,
                filter: "[status] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_Requests_MemberId",
                table: "Registration_Requests",
                column: "MemberId",
                unique: true,
                filter: "[status] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccounts_Email",
                table: "DashboardAccounts",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visitors_Email",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_MemberId",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Registration_Requests_Email",
                table: "Registration_Requests");

            migrationBuilder.DropIndex(
                name: "IX_Registration_Requests_MemberId",
                table: "Registration_Requests");

            migrationBuilder.DropIndex(
                name: "IX_DashboardAccounts_Email",
                table: "DashboardAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Registration_Requests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DashboardAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
