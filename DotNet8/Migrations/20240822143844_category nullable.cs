using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet8.Migrations
{
    /// <inheritdoc />
    public partial class categorynullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalEvents_CalEventCategories_CategoryId",
                table: "CalEvents");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CalEvents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CalEvents_CalEventCategories_CategoryId",
                table: "CalEvents",
                column: "CategoryId",
                principalTable: "CalEventCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalEvents_CalEventCategories_CategoryId",
                table: "CalEvents");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CalEvents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalEvents_CalEventCategories_CategoryId",
                table: "CalEvents",
                column: "CategoryId",
                principalTable: "CalEventCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
