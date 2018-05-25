using Microsoft.EntityFrameworkCore;
using RedisApp.Domain.Entities;

namespace RedisApp.Infrastructure
{
    public class RedisAppContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Producer> Producers { get; set; }

        public RedisAppContext()
        {
        }

        public RedisAppContext(DbContextOptions<RedisAppContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RedisApp;Integrated Security=SSPI;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("RedisApp");
            SetupProductTable(modelBuilder);
            SetupCategoryTable(modelBuilder);
            SetupOrderTable(modelBuilder);
            SetupOrderItemTable(modelBuilder);
            SetupProducerTable(modelBuilder);
        }

        private static void SetupOrderItemTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().HasKey(cl => cl.OrderItemId);
            modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Product).WithMany(p => p.OrderItems).IsRequired();
            modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(p => p.OrderItems).IsRequired();
            modelBuilder.Entity<OrderItem>().Property(oi => oi.Quantity).IsRequired();
            modelBuilder.Entity<OrderItem>().HasIndex(oi => new { oi.ProductId, oi.OrderId }).IsUnique();
        }

        private static void SetupOrderTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>().Property(o => o.ClientName).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.Street).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.City).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.Voivodeship).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.Country).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.Date).IsRequired();
            modelBuilder.Entity<Order>().HasIndex(o => o.Date);
            modelBuilder.Entity<Order>().HasIndex(o => o.ClientName);
        }

        private static void SetupCategoryTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(e => e.CategoryId);
            modelBuilder.Entity<Category>().Property(e => e.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        }

        private static void SetupProductTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(e => e.ProductId);
            modelBuilder.Entity<Product>().HasOne(p => p.Producer).WithMany(p => p.Products).IsRequired();
            modelBuilder.Entity<Product>().Property(e => e.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Product>().Property(e => e.Description).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Product>().Property(e => e.Price).HasColumnType("decimal(8,2)").IsRequired();
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).IsRequired();
            modelBuilder.Entity<Product>()
                        .HasIndex(p => new { p.Name, p.CategoryId })
                        .IsUnique();
        }

        private static void SetupProducerTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producer>().HasKey(p => p.ProducerId);
            modelBuilder.Entity<Producer>().Property(p => p.Name).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Producer>().Property(p => p.CreatedDate).IsRequired();
            modelBuilder.Entity<Producer>().HasIndex(p => p.Name).IsUnique();
        }
    }
}
