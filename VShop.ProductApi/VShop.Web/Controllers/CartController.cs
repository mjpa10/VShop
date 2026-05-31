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

        if (cart?.CartHeader is null)
            return null;

            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode, await GetAcessToken());

                if (coupon is not null)
                {
                    cart.CartHeader.Discount = coupon.Discount;
                }
            }
            if (cart.CartItems is not null && cart.CartItems.Any())
            {
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

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        CartViewModel? cartVM = await GetCartByUser();
        return View(cartVM);
    }
    [HttpPost]
    public async Task<IActionResult> Checkout(CartViewModel cartViewModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _cartService.CheckoutAsync(cartViewModel.CartHeader, await GetAcessToken());

            if (result is not null)
            {
                return RedirectToAction(nameof(CheckoutCompleted));
            }
        }
        return View(cartViewModel);
    }
    [HttpGet]
    public IActionResult CheckoutCompleted()
    {
        var orderNumber = $"{Random.Shared.Next(100, 999)}-{(char)Random.Shared.Next(65, 90)}{Random.Shared.Next(10, 99)}";
        ViewBag.OrderNumber = orderNumber;
        return View();
    }
}
