using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class added_medicalsubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Cases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CaseComments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalSubCategories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalCategory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalSubCategories", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_UserId",
                table: "Cases",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseComments_UserId",
                table: "CaseComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseComments_AspNetUsers_UserId",
                table: "CaseComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_AspNetUsers_UserId",
                table: "Cases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseComments_AspNetUsers_UserId",
                table: "CaseComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_AspNetUsers_UserId",
                table: "Cases");

            migrationBuilder.DropTable(
                name: "MedicalSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Cases_UserId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_CaseComments_UserId",
                table: "CaseComments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CaseComments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
