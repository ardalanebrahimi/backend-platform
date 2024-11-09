using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Ardiland.Models;

namespace Ardiland.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Include the "price" property in the mapping configuration
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(10, 2)"); // Use the appropriate decimal type based on your database

            // Include the "price" property in the mapping configuration
            modelBuilder.Entity<Customer>(); // Use the appropriate decimal type based on your database

            // Other configurations...
        }
    }
}
