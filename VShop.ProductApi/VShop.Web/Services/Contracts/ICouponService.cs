
using VShop.Web.Models;

namespace VShop.Web.Services.Contracts;

public interface ICouponService
{
    Task<CouponViewModel> GetCouponByCodeAsync(string couponCode, string token);
}
