using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class mod_projects_preplans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrePlan",
                table: "Projects");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<Guid>(
                name: "PrePlanId",
                table: "Projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PrePlanId",
                table: "Projects",
                column: "PrePlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects",
                column: "PrePlanId",
                principalTable: "PrePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PrePlanId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PrePlanId",
                table: "Projects");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<string>(
                name: "PrePlan",
                table: "Projects",
                type: "jsonb",
                nullable: true);
        }
    }
}
