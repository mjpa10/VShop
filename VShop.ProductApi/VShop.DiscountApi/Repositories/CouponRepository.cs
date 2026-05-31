using AutoMapper;
using VShop.DiscountApi.Context;
using VShop.DiscountApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace VShop.DiscountApi.Repositories;

public class CouponRepository: ICouponRepository
{
    private readonly AppDbContext _context;
    private IMapper _mapper;

    public CouponRepository(IMapper mapper, AppDbContext context)
    {

        _mapper = mapper;
        _context = context;
    }

    public async Task<CouponDTO> GetCouponByCode(string couponCode)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

        return _mapper.Map<CouponDTO>(coupon);
    }
}
