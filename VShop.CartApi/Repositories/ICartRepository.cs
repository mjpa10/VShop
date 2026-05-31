using VShop.CartApi.DTOs;

namespace VShop.CartApi.Repositories;

public interface ICartRepository 
{
    Task<CartDTO> GetCartByUserIdAsync(string userId);
    Task<CartDTO> UpdateCartAsync(CartDTO cartDto);
    Task<bool> DeleteItemCartAsync(int carItemId);
    Task<bool> CleanCartAsync(string userId);

    Task<bool> ApplyCouponsAsync(string userId, string couponCode);
    Task<bool> DeleteCouponsAsync(string userId);
}
