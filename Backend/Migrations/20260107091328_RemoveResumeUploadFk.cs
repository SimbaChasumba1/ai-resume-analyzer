using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResumeUploadFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.AddColumn<Guid>(
                name: "AnalysisId",
                table: "ResumeUploads",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumeUploads_AnalysisId",
                table: "ResumeUploads",
                column: "AnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeUploads_AIAnalyses_AnalysisId",
                table: "ResumeUploads",
                column: "AnalysisId",
                principalTable: "AIAnalyses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResumeUploads_AIAnalyses_AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.DropIndex(
                name: "IX_ResumeUploads_AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeUploadId",
                table: "AIAnalyses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses",
                column: "ResumeUploadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId",
                table: "AIAnalyses",
                column: "ResumeUploadId",
                principalTable: "ResumeUploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
