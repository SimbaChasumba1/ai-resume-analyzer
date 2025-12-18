using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}

        public DbSet<User> Users => Set<User>();
        public DbSet<ResumeUpload> ResumeUploads => Set<ResumeUpload>();
        public DbSet<AIAnalysis> AIAnalyses => Set<AIAnalysis>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ResumeUpload>()
                .HasOne(r => r.User)
                .WithMany(u => u.Resumes)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<AIAnalysis>()
                .HasOne(a => a.Resume)
                .WithMany(r => r.Analyses)
                .HasForeignKey(a => a.ResumeUploadId);
        }
    }
}
