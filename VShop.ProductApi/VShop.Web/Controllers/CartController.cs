using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        CartViewModel? cartVM = await GetCartByUser();

        if (cartVM == null)
        {
            ModelState.AddModelError("CartNotFound", "Cart does not exist.Come on Shopping...");
            return View("/Views/Cart/CartNotFound.cshtml");
        }
        return View(cartVM);
    }

    public async Task<IActionResult> RemoveItem(int id)
    {
        var result = await _cartService.RemoveItemFromCartAsync(id, await GetAcessToken());
        if (result)
        {
            return RedirectToAction(nameof(Index));

        }
        return View(id);
        }

    private async Task<CartViewModel?> GetCartByUser()
    {
        var userId = GetCartByUserId();
        Console.WriteLine($">>> UserId: {userId}");

        var cart = await _cartService.GetCartByUserIdAsync(GetCartByUserId(), await GetAcessToken());



        if (cart?.CartHeader is not null) 
        {
            foreach (var item in cart.CartItems)
            {
                cart.CartHeader.TotalAmount += (item.Product.Price * item.Quantity);
            }
        }
        return cart;
    }
    private string GetCartByUserId()
    {
        return User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
    }

    private async Task<string> GetAcessToken()
    {
        return await HttpContext.GetTokenAsync("access_token");
    }
}
