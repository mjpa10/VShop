namespace VShop.ProductApi.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public ICollection<Product>? Products { get; set; } // uma categoria pode ter muitos produtos, é uma relação de um para muitos.
}
