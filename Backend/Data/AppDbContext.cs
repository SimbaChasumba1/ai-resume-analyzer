using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<ResumeUpload> ResumeUploads => Set<ResumeUpload>();

        public DbSet<AnalysisJob> AnalysisJobs => Set<AnalysisJob>();

        public DbSet<AIAnalysis> AIAnalyses => Set<AIAnalysis>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ResumeUpload>()
                .HasOne(r => r.User)
                .WithMany(u => u.ResumeUploads)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnalysisJob>()
                .HasOne(j => j.ResumeUpload)
                .WithOne(r => r.AnalysisJob)
                .HasForeignKey<AnalysisJob>(j => j.ResumeUploadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnalysisJob>()
                .HasOne(j => j.User)
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AIAnalysis>()
                .HasOne(a => a.AnalysisJob)
                .WithOne(j => j.Analysis)
                .HasForeignKey<AIAnalysis>(a => a.AnalysisJobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnalysisJob>()
                .HasIndex(j => new { j.UserId, j.CreatedAt });

            modelBuilder.Entity<AnalysisJob>()
                .HasIndex(j => j.Status);

            modelBuilder.Entity<OutboxMessage>()
                .HasIndex(m => new
                {
                    m.ProcessedAt,
                    m.NextAttemptAt,
                    m.CreatedAt
                });
        }
    }
}