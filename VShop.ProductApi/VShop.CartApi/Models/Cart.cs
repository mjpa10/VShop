using System.ComponentModel.DataAnnotations;
using VShop.CartApi.Models;

namespace WingtipToys.Models;

public class Cart
{
    public CartHeader CartHeader { get; set; } = new CartHeader();
    public IEnumerable<CartItem> CartItems { get; set; } = Enumerable.Empty<CartItem>();
}
