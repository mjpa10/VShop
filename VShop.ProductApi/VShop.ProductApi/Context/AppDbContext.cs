using Microsoft.EntityFrameworkCore;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // DbSet é uma coleção de entidades do tipo Product, que representa a tabela de produtos no banco de dados.
    public DbSet<Product>? Products { get; set; }  // o modelo se chama Product, e a tabela se chama Products, é uma convenção do Entity Framework.
    public DbSet<Category>? Categories { get; set; } // o modelo se chama Category, e a tabela se chama Categories, é uma convenção do Entity Framework.

    //fluent API para configurar o relacionamento entre as entidades Product e Category
    protected override void OnModelCreating(ModelBuilder mb)
    {
        // mapeamento da entidade Category(é parecido com data annotations)
        mb.Entity<Category>().HasKey(c => c.CategoryId); // a chave primária da entidade Category é a propriedade CategoryId)

        mb.Entity<Category>().
            Property(c => c.Name).
            IsRequired().
            HasMaxLength(100); // o nome do produto é obrigatório e tem um tamanho máximo de 100 caracteres

        //Product
        mb.Entity<Product>().
            Property(c => c.Name).
            IsRequired().
            HasMaxLength(100);

        mb.Entity<Product>().
          Property(c => c.Description).
          IsRequired().
          HasMaxLength(255);

        mb.Entity<Product>().
         Property(c => c.ImageURL).
         IsRequired().
         HasMaxLength(255);

        mb.Entity<Product>().
            Property(c => c.Price).
            HasPrecision(12, 2); // o preço do produto tem uma precisão de 12 dígitos e 2 casas decimais

        mb.Entity<Category>().
            HasMany(g => g.Products).
            WithOne(c => c.Category).
            IsRequired().
            OnDelete(DeleteBehavior.Cascade);
        // um para muitos, uma categoria tem muitos produtos, um produto tem uma categoria, a exclusão em cascata significa que quando uma categoria for excluída,
        // todos os produtos relacionados a ela também serão excluídos.)

        mb.Entity<Category>().HasData(
            new Category
            {
                CategoryId = 1,
                Name = "Material Escolar"
            },
            new Category
            {
                CategoryId = 2,
                Name = "Acessórios"
            }
         );
    }
}
