using backend.Models;
using Microsoft.EntityFrameworkCore;


namespace backend.Data;


public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}


public DbSet<User> Users => Set<User>();
public DbSet<ResumeUpload> ResumeUploads => Set<ResumeUpload>();
public DbSet<AIAnalysis> AIAnalyses => Set<AIAnalysis>();


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
modelBuilder.Entity<User>()
.HasMany(u => u.ResumeUploads)
.WithOne(r => r.User)
.HasForeignKey(r => r.UserId);


modelBuilder.Entity<ResumeUpload>()
.HasOne(r => r.Analysis)
.WithOne(a => a.Resume)
.HasForeignKey<AIAnalysis>(a => a.ResumeUploadId);
}
}