using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class mod_teams_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrePlans_Teams_TeamId",
                table: "PrePlans");

            migrationBuilder.DropIndex(
                name: "IX_PrePlans_TeamId",
                table: "PrePlans");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamModel",
                table: "PrePlans",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrePlans_TeamModel",
                table: "PrePlans",
                column: "TeamModel");

            migrationBuilder.AddForeignKey(
                name: "FK_PrePlans_Teams_TeamModel",
                table: "PrePlans",
                column: "TeamModel",
                principalTable: "Teams",
                principalColumn: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrePlans_Teams_TeamModel",
                table: "PrePlans");

            migrationBuilder.DropIndex(
                name: "IX_PrePlans_TeamModel",
                table: "PrePlans");

            migrationBuilder.DropColumn(
                name: "TeamModel",
                table: "PrePlans");

            migrationBuilder.CreateIndex(
                name: "IX_PrePlans_TeamId",
                table: "PrePlans",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrePlans_Teams_TeamId",
                table: "PrePlans",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
