using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedLabDashboardAccRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DashboardAccountLab",
                columns: table => new
                {
                    AccessAccountsId = table.Column<int>(type: "int", nullable: false),
                    LabsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardAccountLab", x => new { x.AccessAccountsId, x.LabsId });
                    table.ForeignKey(
                        name: "FK_DashboardAccountLab_DashboardAccounts_AccessAccountsId",
                        column: x => x.AccessAccountsId,
                        principalTable: "DashboardAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardAccountLab_Labs_LabsId",
                        column: x => x.LabsId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccountLab_LabsId",
                table: "DashboardAccountLab",
                column: "LabsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardAccountLab");
        }
    }
}
