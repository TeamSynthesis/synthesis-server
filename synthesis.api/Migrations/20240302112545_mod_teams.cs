using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class mod_teams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Invites",
                table: "Teams",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatsAvailable",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Teams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invites",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SeatsAvailable",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Teams");
        }
    }
}
