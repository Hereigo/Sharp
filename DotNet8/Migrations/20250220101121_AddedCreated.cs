using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendarium.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "CalEvents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "CalEvents");
        }
    }
}
