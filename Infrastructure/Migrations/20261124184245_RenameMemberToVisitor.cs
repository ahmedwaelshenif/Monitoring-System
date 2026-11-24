using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMemberToVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs");

            migrationBuilder.DropTable(
                name: "LabMember");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "EntryLogs",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "EntryLogs",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLogs_MemberId",
                table: "EntryLogs",
                newName: "IX_EntryLogs_VisitorId");

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    FaceImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IdImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    EmbeddingData = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabVisitor",
                columns: table => new
                {
                    LabsId = table.Column<int>(type: "int", nullable: false),
                    VisitorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabVisitor", x => new { x.LabsId, x.VisitorsId });
                    table.ForeignKey(
                        name: "FK_LabVisitor_Labs_LabsId",
                        column: x => x.LabsId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabVisitor_Visitors_VisitorsId",
                        column: x => x.VisitorsId,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabVisitor_VisitorsId",
                table: "LabVisitor",
                column: "VisitorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Visitors_VisitorId",
                table: "EntryLogs",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Visitors_VisitorId",
                table: "EntryLogs");

            migrationBuilder.DropTable(
                name: "LabVisitor");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "EntryLogs",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "EntryLogs",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLogs_VisitorId",
                table: "EntryLogs",
                newName: "IX_EntryLogs_MemberId");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmbeddingData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FaceImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabMember",
                columns: table => new
                {
                    LabsId = table.Column<int>(type: "int", nullable: false),
                    MembersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabMember", x => new { x.LabsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_LabMember_Labs_LabsId",
                        column: x => x.LabsId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabMember_Members_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabMember_MembersId",
                table: "LabMember",
                column: "MembersId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
