using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class dataremodeling_cases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalSubCategoryId",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Cases_MedicalSubCategoryId",
                table: "Cases",
                column: "MedicalSubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_MedicalSubCategories_MedicalSubCategoryId",
                table: "Cases",
                column: "MedicalSubCategoryId",
                principalTable: "MedicalSubCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_MedicalSubCategories_MedicalSubCategoryId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_MedicalSubCategoryId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "MedicalSubCategoryId",
                table: "Cases");
        }
    }
}
