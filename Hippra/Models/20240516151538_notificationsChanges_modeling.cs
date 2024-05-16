using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippra.Migrations
{
    /// <inheritdoc />
    public partial class notificationsChanges_modeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderUserID",
                table: "Notifications",
                newName: "SenderUserId");

            migrationBuilder.RenameColumn(
                name: "PostID",
                table: "Notifications",
                newName: "CaseId");

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CaseId",
                table: "Notifications",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderUserId",
                table: "Notifications",
                column: "SenderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderUserId",
                table: "Notifications",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Cases_CaseId",
                table: "Notifications",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Cases_CaseId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CaseId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SenderUserId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "Notifications",
                newName: "SenderUserID");

            migrationBuilder.RenameColumn(
                name: "CaseId",
                table: "Notifications",
                newName: "PostID");

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserID",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
