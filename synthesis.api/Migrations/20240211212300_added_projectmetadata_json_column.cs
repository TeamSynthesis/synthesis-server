using Microsoft.EntityFrameworkCore.Migrations;
using synthesis.api.Data.Models;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class added_projectmetadata_json_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");

            migrationBuilder.AddColumn<ProjectMetadata>(
                name: "ProjectMetadata",
                table: "Projects",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectMetadata",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "text",
                nullable: true);
        }
    }
}
