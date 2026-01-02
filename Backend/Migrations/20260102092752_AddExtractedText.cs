using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddExtractedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ParsedText",
                table: "ResumeUploads",
                newName: "FilePath");

            migrationBuilder.AddColumn<string>(
                name: "ExtractedText",
                table: "ResumeUploads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeUploadId1",
                table: "AIAnalyses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_ResumeUploadId1",
                table: "AIAnalyses",
                column: "ResumeUploadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId1",
                table: "AIAnalyses",
                column: "ResumeUploadId1",
                principalTable: "ResumeUploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId1",
                table: "AIAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_ResumeUploadId1",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "ExtractedText",
                table: "ResumeUploads");

            migrationBuilder.DropColumn(
                name: "ResumeUploadId1",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ResumeUploads",
                newName: "ParsedText");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}
