using LegoMastersPlus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace LegoMastersPlus.Data
{
    public class LegoMastersDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public LegoMastersDbContext(DbContextOptions<LegoMastersDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<ProductItemRecommendation> ProductItemRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("Customers");
            //modelBuilder.Entity<Admin>().ToTable("Admins");
        }


    }
}
