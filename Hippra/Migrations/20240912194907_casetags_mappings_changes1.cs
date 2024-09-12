using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class casetags_mappings_changes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseTag_Tags_TagsID",
                table: "CaseTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseTag",
                table: "CaseTag");

            migrationBuilder.DropIndex(
                name: "IX_CaseTag_TagsID",
                table: "CaseTag");

            migrationBuilder.RenameColumn(
                name: "TagsID",
                table: "CaseTag",
                newName: "CaseTagsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseTag",
                table: "CaseTag",
                columns: new[] { "CaseTagsID", "CasesID" });

            migrationBuilder.CreateIndex(
                name: "IX_CaseTag_CasesID",
                table: "CaseTag",
                column: "CasesID");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseTag_Tags_CaseTagsID",
                table: "CaseTag",
                column: "CaseTagsID",
                principalTable: "Tags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseTag_Tags_CaseTagsID",
                table: "CaseTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseTag",
                table: "CaseTag");

            migrationBuilder.DropIndex(
                name: "IX_CaseTag_CasesID",
                table: "CaseTag");

            migrationBuilder.RenameColumn(
                name: "CaseTagsID",
                table: "CaseTag",
                newName: "TagsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseTag",
                table: "CaseTag",
                columns: new[] { "CasesID", "TagsID" });

            migrationBuilder.CreateIndex(
                name: "IX_CaseTag_TagsID",
                table: "CaseTag",
                column: "TagsID");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseTag_Tags_TagsID",
                table: "CaseTag",
                column: "TagsID",
                principalTable: "Tags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
