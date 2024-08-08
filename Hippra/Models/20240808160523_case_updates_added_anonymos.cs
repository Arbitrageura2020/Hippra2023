using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class case_updates_added_anonymos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseNeeded",
                table: "Cases");

            migrationBuilder.AddColumn<bool>(
                name: "PostedAnonymosley",
                table: "Cases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CaseLikes_CaseId",
                table: "CaseLikes",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseLikes_Cases_CaseId",
                table: "CaseLikes",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseLikes_Cases_CaseId",
                table: "CaseLikes");

            migrationBuilder.DropIndex(
                name: "IX_CaseLikes_CaseId",
                table: "CaseLikes");

            migrationBuilder.DropColumn(
                name: "PostedAnonymosley",
                table: "Cases");

            migrationBuilder.AddColumn<int>(
                name: "ResponseNeeded",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
