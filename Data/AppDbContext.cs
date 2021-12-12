using Microsoft.EntityFrameworkCore;
using prodotnet.Models;

namespace prodotnet.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext( DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>(entity => {
                entity.HasIndex(p => p.Slug);
            });
            modelBuilder.Entity<PostCategory>().HasKey(p => new {p.PostID, p.CategoryID});
            modelBuilder.Entity<Post>(entity => {
                entity.HasIndex(p => p.Slug);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories {set; get;}
        public DbSet<Post> Posts {set; get;}
        public DbSet<PostCategory> PostCategories {set; get;}
        public DbSet<Role> Roles {get;set;}
    }
}