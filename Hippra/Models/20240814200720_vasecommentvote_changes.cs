using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class vasecommentvote_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterID",
                table: "CaseCommentVotes");

            migrationBuilder.DropColumn(
                name: "VoterID",
                table: "CaseCommentVotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosterID",
                table: "CaseCommentVotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoterID",
                table: "CaseCommentVotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
