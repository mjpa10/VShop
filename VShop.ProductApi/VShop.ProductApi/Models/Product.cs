namespace VShop.ProductApi.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public long Stock { get; set; }
    public string? ImageURL { get; set; }  
    public Category? Category { get; set; } // quer dizer que cada produto tem uma categoria, é uma relação de um para muitos.
    public int CategoryId { get; set; }

}
