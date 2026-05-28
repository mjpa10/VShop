using Microsoft.EntityFrameworkCore;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<DbContext> options) : base(options) { }
    // DbSet é uma coleção de entidades do tipo Product, que representa a tabela de produtos no banco de dados.
    public DbSet<Product>? Products { get; set; }  // o modelo se chama Product, e a tabela se chama Products, é uma convenção do Entity Framework.
    public DbSet<Category>? Categories { get; set; } // o modelo se chama Category, e a tabela se chama Categories, é uma convenção do Entity Framework.

}
