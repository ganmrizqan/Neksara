using Microsoft.EntityFrameworkCore;
using Neksara.Models;

namespace Neksara.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TopicView> TopicViews { get; set; }
        public DbSet<SearchLog> SearchLogs { get; set; }    
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Topic)
                .WithMany(t => t.Feedbacks)
                .HasForeignKey(f => f.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Topic>().ToTable("Topics");
            modelBuilder.Entity<Feedback>().ToTable("Feedbacks");
            modelBuilder.Entity<TopicView>().ToTable("TopicViews");
            modelBuilder.Entity<SearchLog>().ToTable("SearchLogs");
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<ContactMessage>().ToTable("ContactMessages");
        }
    
    }
}