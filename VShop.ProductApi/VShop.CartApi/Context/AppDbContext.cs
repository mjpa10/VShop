using Microsoft.EntityFrameworkCore;
using VShop.CartApi.DTOs;
using VShop.CartApi.Models;
using WingtipToys.Models;

namespace VShop.ProductApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<CartItem>? CartItems { get; set; }
    public DbSet<CartHeader>? CartHeaders { get; set; }

    //fluent API para configurar o relacionamento entre as entidades Product e Category
    protected override void OnModelCreating(ModelBuilder mb)
    {
        //Product
        mb.Entity<Product>()
            .HasKey(c => c.Id);

        //Product
        mb.Entity<Product>().
           Property(c => c.Id)
            .ValueGeneratedNever();

        mb.Entity<Product>().
           Property(c => c.Name).
             HasMaxLength(100).
               IsRequired();

        mb.Entity<Product>().
          Property(c => c.Description).
               HasMaxLength(255).
                   IsRequired();

        mb.Entity<Product>().
          Property(c => c.ImageURL).
              HasMaxLength(255).
                  IsRequired();

        mb.Entity<Product>().
           Property(c => c.CategoryName).
               HasMaxLength(100).
                IsRequired();

        mb.Entity<Product>().
           Property(c => c.Price).
             HasPrecision(12, 2);

        //CartHeader
        mb.Entity<CartHeader>().
             Property(c => c.UserId).
             HasMaxLength(255).
                 IsRequired();

        mb.Entity<CartHeader>().
           Property(c => c.CouponCode).
              HasMaxLength(100);
    }
}
