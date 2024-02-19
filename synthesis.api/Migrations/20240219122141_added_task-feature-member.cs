using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace synthesis.api.Migrations
{
    /// <inheritdoc />
    public partial class added_taskfeaturemember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Features_FeatureModelId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Members_MemberModelId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "MemberModelId",
                table: "Tasks",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "FeatureModelId",
                table: "Tasks",
                newName: "FeatureId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_MemberModelId",
                table: "Tasks",
                newName: "IX_Tasks_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_FeatureModelId",
                table: "Tasks",
                newName: "IX_Tasks_FeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Features_FeatureId",
                table: "Tasks",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "FeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Members_MemberId",
                table: "Tasks",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Features_FeatureId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Members_MemberId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Tasks",
                newName: "MemberModelId");

            migrationBuilder.RenameColumn(
                name: "FeatureId",
                table: "Tasks",
                newName: "FeatureModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_MemberId",
                table: "Tasks",
                newName: "IX_Tasks_MemberModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_FeatureId",
                table: "Tasks",
                newName: "IX_Tasks_FeatureModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Features_FeatureModelId",
                table: "Tasks",
                column: "FeatureModelId",
                principalTable: "Features",
                principalColumn: "FeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Members_MemberModelId",
                table: "Tasks",
                column: "MemberModelId",
                principalTable: "Members",
                principalColumn: "MemberId");
        }
    }
}
