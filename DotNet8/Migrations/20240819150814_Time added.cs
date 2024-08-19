using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet8.Migrations
{
    /// <inheritdoc />
    public partial class Timeadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "CalEvents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "CalEvents");
        }
    }
}
