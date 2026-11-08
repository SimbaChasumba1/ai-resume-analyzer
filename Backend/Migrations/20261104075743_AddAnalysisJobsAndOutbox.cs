using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisJobsAndOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_Users_UserId",
                table: "AIAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeUploads_AIAnalyses_AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.DropIndex(
                name: "IX_ResumeUploads_AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_UserId",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "ResumeUploads");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AIAnalyses",
                newName: "AnalysisJobId");

            migrationBuilder.CreateTable(
                name: "AnalysisJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeUploadId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisJobs_ResumeUploads_ResumeUploadId",
                        column: x => x.ResumeUploadId,
                        principalTable: "ResumeUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisJobs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextAttemptAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalyses_AnalysisJobId",
                table: "AIAnalyses",
                column: "AnalysisJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_ResumeUploadId",
                table: "AnalysisJobs",
                column: "ResumeUploadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_Status",
                table: "AnalysisJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisJobs_UserId_CreatedAt",
                table: "AnalysisJobs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt_NextAttemptAt_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAt", "NextAttemptAt", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_AnalysisJobs_AnalysisJobId",
                table: "AIAnalyses",
                column: "AnalysisJobId",
                principalTable: "AnalysisJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_AnalysisJobs_AnalysisJobId",
                table: "AIAnalyses");

            migrationBuilder.DropTable(
                name: "AnalysisJobs");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_AIAnalyses_AnalysisJobId",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "AnalysisJobId",
                table: "AIAnalyses",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "AnalysisId",
                table: "ResumeUploads",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumeUploads_AnalysisId",
                table: "ResumeUploads",
                column: "AnalysisId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeUploads_AIAnalyses_AnalysisId",
                table: "ResumeUploads",
                column: "AnalysisId",
                principalTable: "AIAnalyses",
                principalColumn: "Id");
        }
    }
}
