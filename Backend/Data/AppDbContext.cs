using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ResumeUpload> ResumeUploads { get; set; }

        // Rename to match your controller usage
        public DbSet<ResumeAnalysis> ResumeAnalyses { get; set; }

        // Optional: keep AIAnalyses if you want separate tracking
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-one relationship between ResumeUpload and AIAnalysis
            modelBuilder.Entity<ResumeUpload>()
                .HasOne(r => r.Analysis)
                .WithOne(a => a.Resume)
                .HasForeignKey<AIAnalysis>(a => a.ResumeUploadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
