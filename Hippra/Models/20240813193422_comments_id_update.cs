using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class comments_id_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Cases_CaseID",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "PostTags",
                newName: "TagID");

            migrationBuilder.RenameIndex(
                name: "IX_PostTags_TagId",
                table: "PostTags",
                newName: "IX_PostTags_TagID");

            migrationBuilder.AlterColumn<int>(
                name: "TagID",
                table: "PostTags",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CaseID",
                table: "PostTags",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CasesID",
                table: "PostTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagsId",
                table: "PostTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "CommentId",
                table: "Notifications",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Cases_CaseID",
                table: "PostTags",
                column: "CaseID",
                principalTable: "Cases",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagID",
                table: "PostTags",
                column: "TagID",
                principalTable: "Tags",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Cases_CaseID",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagID",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "CasesID",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "TagsId",
                table: "PostTags");

            migrationBuilder.RenameColumn(
                name: "TagID",
                table: "PostTags",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_PostTags_TagID",
                table: "PostTags",
                newName: "IX_PostTags_TagId");

            migrationBuilder.AlterColumn<int>(
                name: "TagId",
                table: "PostTags",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaseID",
                table: "PostTags",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Cases_CaseID",
                table: "PostTags",
                column: "CaseID",
                principalTable: "Cases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
