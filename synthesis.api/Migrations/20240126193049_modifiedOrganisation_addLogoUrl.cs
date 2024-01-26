using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class modifiedOrganisation_addLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Organisations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Organisations");
        }
    }
}
