using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    private readonly ICouponService _couponService;


    public CartController(ICartService cartService, ICouponService couponService)
    {
        _cartService = cartService;
        _couponService = couponService;
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
        var cart = await _cartService.GetCartByUserIdAsync(GetCartByUserId(), await GetAcessToken());
        var teste = await _couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode, await GetAcessToken());

        Console.WriteLine($">>> CouponCode: {cart.CartHeader.CouponCode}");
        Console.WriteLine($">>> Coupon: {teste?.CouponCode ?? "NULL"}");
        Console.WriteLine($">>> Discount: {teste?.Discount}");

        if (teste is not null)
        {
            cart.CartHeader.Discount = teste.Discount;
            Console.WriteLine($">>> Discount aplicado: {cart.CartHeader.Discount}");
        }

        if (cart?.CartHeader is not null)
        {
            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode, await GetAcessToken());

                if (coupon is not null)
                {
                    cart.CartHeader.Discount = coupon.Discount;
                }
            }
            foreach (var item in cart.CartItems)
            {
                cart.CartHeader.TotalAmount += (item.Product.Price * item.Quantity);
            }
            cart.CartHeader.TotalAmount -= (cart.CartHeader.TotalAmount * cart.CartHeader.Discount) / 100;
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

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartViewModel cartVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _cartService.ApplyCouponAsync(cartVM, await GetAcessToken());

            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return RedirectToAction(nameof(Index)); ;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCoupon()
    {
        var result = await _cartService.RemoveCouponAsync(GetCartByUserId(), await GetAcessToken());

        if (result)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index)); ;
    }
}
