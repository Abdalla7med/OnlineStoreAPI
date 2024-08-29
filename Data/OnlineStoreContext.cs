using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer;
using Models;
using System.Reflection.Emit;

namespace Data
{
    public class OnlineStoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
        public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options)
        : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=OnlineStore;Integrated Security=True;TrustServerCertificate=True");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            // Configure many-to-many relationship between Order and Product

            modelBuilder.Entity<OrderDetail>()
           .HasKey(od => new {od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Product>().
                HasKey(P => P.Id);
              
            modelBuilder.Entity<Order>()
           .Property(o => o.OrderDate)
           .HasDefaultValueSql("GETDATE()"); // For SQL Server default date


            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<int>(); // Store enum as integer in the database

            
           

           
            base.OnModelCreating(modelBuilder);

        }
    }
}
