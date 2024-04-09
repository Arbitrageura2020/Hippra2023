using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class added_medicalcategory_as_enum_to_case : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalCategory1",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalCategory1",
                table: "Cases");
        }
    }
}
