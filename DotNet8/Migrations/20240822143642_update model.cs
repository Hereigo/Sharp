using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet8.Migrations
{
    /// <inheritdoc />
    public partial class updatemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EveryXdays",
                table: "CalEvents",
                newName: "EveryXDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EveryXDays",
                table: "CalEvents",
                newName: "EveryXdays");
        }
    }
}
