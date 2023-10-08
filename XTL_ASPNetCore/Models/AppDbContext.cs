                                      using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XTL_ASPNetCore.Models.Blog;
using XTL_ASPNetCore.Models.Product;

namespace XTL_ASPNetCore.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("MyDbContext");
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique(); //IsUnique chi co 1 slug duy nhat
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c =>  new {c.PostID,c.CategoryID});
            });
            modelBuilder.Entity<Post>( entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<CategoryProduct>(entity => {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            modelBuilder.Entity<ProductCategoryProduct>(entity => {
                entity.HasKey(c => new { c.ProductID, c.CategoryID });
            });

            modelBuilder.Entity<ProductModel>(entity => {
                entity.HasIndex(p => p.Slug)
                      .IsUnique();
            });

        }

        public DbSet<Contact> contacts { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Post> Posts { get; set; }

        public DbSet<PostCategory> PostCategories { get; set; }


        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductModel> Products { get; set; }

        public DbSet<ProductCategoryProduct> ProductCategoryProducts { get; set; }

        public DbSet<ProductPhoto> ProductPhotos { get; set; }


    }
}
