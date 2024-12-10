using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectStackOverflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class Content : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentContent",
                table: "Comments",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "AnswerContent",
                table: "Answers",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Comments",
                newName: "CommentContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Answers",
                newName: "AnswerContent");
        }
    }
}
