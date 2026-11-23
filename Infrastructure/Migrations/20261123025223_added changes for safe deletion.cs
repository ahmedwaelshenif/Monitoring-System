using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedchangesforsafedeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Labs_LabId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Warnings_Labs_LabId",
                table: "Warnings");

            migrationBuilder.AlterColumn<int>(
                name: "LabId",
                table: "Warnings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Labs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "LabId",
                table: "EntryLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Labs_LabId",
                table: "EntryLogs",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Warnings_Labs_LabId",
                table: "Warnings",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Labs_LabId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Warnings_Labs_LabId",
                table: "Warnings");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Labs");

            migrationBuilder.AlterColumn<int>(
                name: "LabId",
                table: "Warnings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LabId",
                table: "EntryLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Labs_LabId",
                table: "EntryLogs",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Members_MemberId",
                table: "EntryLogs",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Warnings_Labs_LabId",
                table: "Warnings",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
