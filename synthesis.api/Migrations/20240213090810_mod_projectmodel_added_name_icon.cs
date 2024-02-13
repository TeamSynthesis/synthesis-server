using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class mod_projectmodel_added_name_icon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");
        }
    }
}
