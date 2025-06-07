using Microsoft.EntityFrameworkCore;
using CornerShop.Models;

namespace CornerShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Total).HasPrecision(18, 2);
                entity.HasMany(e => e.Items)
                    .WithOne()
                    .HasForeignKey("SaleId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });
        }
    }
}
