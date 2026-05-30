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
}
