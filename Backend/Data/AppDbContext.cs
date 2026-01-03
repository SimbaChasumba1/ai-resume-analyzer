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

        public DbSet<User> Users { get; set; }
        public DbSet<ResumeUpload> ResumeUploads { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-one: ResumeUpload <-> AIAnalysis (GUID FK -> GUID PK)
            modelBuilder.Entity<ResumeUpload>()
                .HasOne(r => r.Analysis)
                .WithOne(a => a.ResumeUpload)
                .HasForeignKey<AIAnalysis>(a => a.ResumeUploadId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> ResumeUpload handled by convention via ResumeUpload.UserId
            // (no navigation required on User)
        }
    }
}
