using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class _nullable_preplanId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrePlanId",
                table: "Projects",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects",
                column: "PrePlanId",
                principalTable: "PrePlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrePlanId",
                table: "Projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PrePlans_PrePlanId",
                table: "Projects",
                column: "PrePlanId",
                principalTable: "PrePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
