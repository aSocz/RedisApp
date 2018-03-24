using Microsoft.EntityFrameworkCore;
using SportsStore.Domain.Entities;

namespace RedisApp.Infrastructure
{
    public class RedisAppContext : DbContext
    {
        public RedisAppContext(DbContextOptions<RedisAppContext> options)
            : base(options)
        { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("RedisApp");
            SetupProductTable(modelBuilder);
            SetupCategoryTable(modelBuilder);
            SetupOrderTable(modelBuilder);
            SetupOrderItemTable(modelBuilder);
        }

        private static void SetupOrderItemTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().HasKey(cl => cl.OrderItemId);
            modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Product).WithMany(p => p.OrderItems).IsRequired();
            modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(p => p.Items).IsRequired();
        }

        private static void SetupOrderTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>().Property(o => o.ClientName).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.Street).HasMaxLength(20);
            modelBuilder.Entity<Order>().Property(o => o.City).HasMaxLength(20);
            modelBuilder.Entity<Order>().Property(o => o.Voivodeship).HasMaxLength(20);
        }

        private static void SetupCategoryTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(e => e.CategoryId);
            modelBuilder.Entity<Category>().Property(e => e.Name).HasMaxLength(50).IsRequired();
        }

        private static void SetupProductTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(e => e.ProductId);
            modelBuilder.Entity<Product>().Property(e => e.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Product>().Property(e => e.Price).HasColumnType("decimal(5,2)").IsRequired();
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).IsRequired();
        }
    }
}
