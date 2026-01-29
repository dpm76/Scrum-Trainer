using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrumTrainer.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIdAndNullableUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_AspNetUsers_UserId",
                table: "QuizResults");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuizResults",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "QuizResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_AspNetUsers_UserId",
                table: "QuizResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_AspNetUsers_UserId",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "QuizResults");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuizResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_AspNetUsers_UserId",
                table: "QuizResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
