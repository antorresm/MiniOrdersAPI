using Microsoft.EntityFrameworkCore;
using MiniOrdersAPI.Domain.Entities;

namespace MiniOrdersAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Lines)
                .WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);
        }
    }
}
