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

        public DbSet<ResumeAnalysis> ResumeAnalyses { get; set; } = null!;
        public DbSet<ResumeUpload> ResumeUploads { get; set; } = null!;
        public DbSet<AIAnalysis> AIAnalyses { get; set; } = null!;
    }
}
