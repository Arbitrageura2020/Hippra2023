using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class update_appuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNPIN",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IdNumber",
                table: "AspNetUsers",
                newName: "NPIN");

            migrationBuilder.AddColumn<int>(
                name: "IDMe",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDMe",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "NPIN",
                table: "AspNetUsers",
                newName: "IdNumber");

            migrationBuilder.AddColumn<bool>(
                name: "IsNPIN",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
