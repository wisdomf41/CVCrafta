using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Model;

namespace ResumeApp.Server.Data
{
    public class ResumeDbContext : DbContext
    {

        public ResumeDbContext(DbContextOptions<ResumeDbContext> options) : base(options)
        {

        }

        public object Resume { get; internal set; }

        // Define DbSet properties for your entities here
        public DbSet<Resume> Resumes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here if needed
            modelBuilder.Entity<Resume>()
                .HasData(
                    new Resume { Id = 1, Name = "Future Ibeche", Title = "C# /.NET Developer", Email = "wisdom41@gmail.com" }

                );              // Assuming Id is the primary key for Resume

        }
    }

}
