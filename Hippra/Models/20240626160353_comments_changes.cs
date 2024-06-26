using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class comments_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterId",
                table: "CaseComments");

            migrationBuilder.DropColumn(
                name: "PosterName",
                table: "CaseComments");

            migrationBuilder.DropColumn(
                name: "VoteDown",
                table: "CaseComments");

            migrationBuilder.DropColumn(
                name: "imgUrl",
                table: "CaseComments");

            migrationBuilder.DropColumn(
                name: "posterSpeciality",
                table: "CaseComments");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymus",
                table: "CaseComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymus",
                table: "CaseComments");

            migrationBuilder.AddColumn<int>(
                name: "PosterId",
                table: "CaseComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PosterName",
                table: "CaseComments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteDown",
                table: "CaseComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "imgUrl",
                table: "CaseComments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "posterSpeciality",
                table: "CaseComments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
