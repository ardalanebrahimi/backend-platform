using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YourNamespace.Models;

namespace YourNamespace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Include the "price" property in the mapping configuration
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(10, 2)"); // Use the appropriate decimal type based on your database

            // Other configurations...
        }
    }
}
