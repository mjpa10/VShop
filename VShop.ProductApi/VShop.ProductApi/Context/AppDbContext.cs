using Microsoft.EntityFrameworkCore;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<DbContext> options) : base(options) { }
    // DbSet é uma coleção de entidades do tipo Product, que representa a tabela de produtos no banco de dados.
    public DbSet<Product>? Products { get; set; }  // o modelo se chama Product, e a tabela se chama Products, é uma convenção do Entity Framework.
    public DbSet<Category>? Categories { get; set; } // o modelo se chama Category, e a tabela se chama Categories, é uma convenção do Entity Framework.

    //fluent API para configurar o relacionamento entre as entidades Product e Category
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // mapeamento da entidade Product(é parecido com data annotations)
        modelBuilder.Entity<Category>().
            Property(c => c.Name).
            IsRequired().
            HasMaxLength(100); // o nome do produto é obrigatório e tem um tamanho máximo de 100 caracteres

        modelBuilder.Entity<Product>().
            Property(c => c.Price).
            HasPrecision(12, 2); // o preço do produto tem uma precisão de 12 dígitos e 2 casas decimais
    }
}
