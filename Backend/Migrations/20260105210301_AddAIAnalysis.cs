using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAIAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Result",
                table: "AIAnalyses",
                newName: "WeaknessesJson");

            migrationBuilder.AddColumn<int>(
                name: "AtsScore",
                table: "AIAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImprovementsJson",
                table: "AIAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "AIAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrengthsJson",
                table: "AIAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "AIAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AIAnalyses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_UserId",
                table: "AIAnalyses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_Users_UserId",
                table: "AIAnalyses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_Users_UserId",
                table: "AIAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_UserId",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "AtsScore",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "ImprovementsJson",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "StrengthsJson",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "WeaknessesJson",
                table: "AIAnalyses",
                newName: "Result");
        }
    }
}
