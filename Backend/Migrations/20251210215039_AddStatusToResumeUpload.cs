using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToResumeUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeId",
                table: "AIAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeUploads_User_UserId",
                table: "ResumeUploads");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.RenameColumn(
                name: "ResumeId",
                table: "AIAnalyses",
                newName: "ResumeUploadId");

            migrationBuilder.RenameIndex(
                name: "IX_AIAnalyses_ResumeId",
                table: "AIAnalyses",
                newName: "IX_AIAnalyses_ResumeUploadId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId",
                table: "AIAnalyses",
                column: "ResumeUploadId",
                principalTable: "ResumeUploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeUploads_Users_UserId",
                table: "ResumeUploads",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeUploadId",
                table: "AIAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResumeUploads_Users_UserId",
                table: "ResumeUploads");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "ResumeUploadId",
                table: "AIAnalyses",
                newName: "ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_AIAnalyses_ResumeUploadId",
                table: "AIAnalyses",
                newName: "IX_AIAnalyses_ResumeId");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AIAnalyses_ResumeUploads_ResumeId",
                table: "AIAnalyses",
                column: "ResumeId",
                principalTable: "ResumeUploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeUploads_User_UserId",
                table: "ResumeUploads",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
