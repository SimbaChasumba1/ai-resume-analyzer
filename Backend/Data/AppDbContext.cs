using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<ResumeUpload> ResumeUploads { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

  
            modelBuilder.Entity<User>()
                .HasMany(u => u.Resumes)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

         
            modelBuilder.Entity<ResumeUpload>()
                .HasOne(r => r.Analysis)
                .WithOne(a => a.Resume)
                .HasForeignKey<AIAnalysis>(a => a.ResumeUploadId);
        }
    }
}
