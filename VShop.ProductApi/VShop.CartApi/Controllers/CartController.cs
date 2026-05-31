using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.CartApi.DTOs;
using VShop.CartApi.Repositories;

namespace VShop.CartApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository _repository;

    public CartController(ICartRepository cartRepository)
    {
        _repository = cartRepository;
    }
    [HttpGet("getcart/{id}")]
    public async Task<ActionResult<CartDTO>> GetByUserId(string id)
    {
        var cartDto = await _repository.GetCartByUserIdAsync(id);
        if (cartDto == null)
        {
            return NotFound();
        }
        return Ok(cartDto);
    }

    [HttpPost("addcart")]
    public async Task<ActionResult<CartDTO>> AddCart(CartDTO cartDto)
    {
        var cart = await _repository.UpdateCartAsync(cartDto);

        if (cart == null)
        {
            return NotFound();
        }
        return Ok(cart);
    }

    [HttpPut("updatecart")]
    public async Task<ActionResult<CartDTO>> UpdateCart(CartDTO cartDto)
    {
        var cart = await _repository.UpdateCartAsync(cartDto);

        if (cart == null)
        {
            return NotFound();
        }
        return Ok(cart);
    }

    [HttpDelete("deletecart/{id}")]
    public async Task<ActionResult<CartDTO>> DeleteCart(int id)
    {
        var status = await _repository.DeleteItemCartAsync(id);
        if (!status) 
            return BadRequest();

        return Ok(status);
    }

    [HttpPost("applycoupon")]
    public async Task<ActionResult<CartDTO>> ApplyCoupon(CartDTO cartDto)
    {
        var result = await _repository.ApplyCouponsAsync(cartDto.CartHeader.UserId, 
            cartDto.CartHeader.CouponCode);

        if (!result)
            return BadRequest($"CartHeader not found for userId = {cartDto.CartHeader.UserId}");
        return Ok(result);
    }

    [HttpDelete("deletecoupon/{userId}")]
    public async Task<ActionResult<CartDTO>> DeleteCoupon(string userId)
    {
        var result = await _repository.DeleteCouponsAsync(userId);

        if (!result)
            return BadRequest($"CartHeader not found for userId = {userId}");
        return Ok(result);
    }
}
