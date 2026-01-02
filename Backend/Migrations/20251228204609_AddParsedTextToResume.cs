using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddParsedTextToResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumeAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "Recommendations",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ResumeUploads",
                newName: "ParsedText");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ResumeUploads",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Weaknesses",
                table: "AIAnalyses",
                newName: "ResultJson");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AIAnalyses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses",
                column: "ResumeUploadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "ParsedText",
                table: "ResumeUploads",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "ResumeUploads",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "ResultJson",
                table: "AIAnalyses",
                newName: "Weaknesses");

            migrationBuilder.AddColumn<string>(
                name: "Recommendations",
                table: "AIAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
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

            migrationBuilder.CreateTable(
                name: "ResumeAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnalysisResult = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResumeText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeAnalyses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses",
                column: "ResumeUploadId",
                unique: true);
        }
    }
}
