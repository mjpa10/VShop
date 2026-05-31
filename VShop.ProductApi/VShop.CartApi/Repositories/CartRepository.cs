using AutoMapper;
using VShop.CartApi.DTOs;
using VShop.ProductApi.Context;
using WingtipToys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;
using VShop.CartApi.Models;

namespace VShop.CartApi.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    private IMapper _mapper;

    public CartRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CartDTO> GetCartByUserIdAsync(string userId)
    {
        Cart cart = new Cart
        {
            //obter o header do carrinho
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId)
        };

        if (cart.CartHeader is null)
            return null;

        //obter os iutens do carrinho
        cart.CartItems = _context.CartItems.Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(c => c.Product);

        return _mapper.Map<CartDTO>(cart);
    }

    public async Task<bool> DeleteItemCartAsync(int carItemId)
    {
        try
        {
            CartItem cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == carItemId);

            int total = _context.CartItems.Where(c => c.CartHeaderId == cartItem.CartHeaderId).Count();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            if (total == 1)
            {
                var cartHeaderRemove = await _context.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartItem.CartHeaderId);
                _context.CartHeaders.Remove(cartHeaderRemove);

                await _context.SaveChangesAsync();
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    } 
    
    public async Task<bool> CleanCartAsync(string userId)
    {
        var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeader is not null)
        {
            _context.CartItems.RemoveRange(_context.CartItems.Where(c => c.CartHeaderId == cartHeader.Id));

            _context.CartHeaders.Remove(cartHeader);

            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    } 
    public async Task<CartDTO> UpdateCartAsync(CartDTO cartDto)
    {
        Cart cart = _mapper.Map<Cart>(cartDto);

        //salva o Produto se não existir
        await SaveProductInDataBase(cartDto, cart);

        //verifica se CartHeader é null
        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader is null)
        {
            //cria um novo carrinho
            await CreateCartHeaderAndItems(cart);
        }
        else
        {
            //atualiza o carrinho
            await UpdateQuantyAndItems(cartDto,cart,cartHeader);
        }
        return _mapper.Map<CartDTO>(cart);
    }

    private async Task UpdateQuantyAndItems(CartDTO cartDto, Cart cart, CartHeader? cartHeader)
    {
        //se CartHeader nao e null, verifica se cartItem possui o mesmo produto, se tiver atualiza a quantidade, senao cria um novo
        var carDetail = await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(
            p => p.ProductId == cart.CartItems.FirstOrDefault().ProductId && p.CartHeaderId == cartHeader.Id);

        if (carDetail is null)
        {
            //cria um novo cartItem
            cart.CartItems.FirstOrDefault().CartHeaderId = cartHeader.Id;
            cart.CartItems.FirstOrDefault().Product = null;
            _context.CartItems.Add(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
        else
        {
            //atualiza a quantidade
            cart.CartItems.FirstOrDefault().Product = null;
            cart.CartItems.FirstOrDefault().Quantity += carDetail.Quantity;
            cart.CartItems.FirstOrDefault().Id = carDetail.Id;
            cart.CartItems.FirstOrDefault().CartHeaderId = carDetail.CartHeaderId;
            _context.CartItems.Update(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
    }

    private async Task CreateCartHeaderAndItems(Cart cart)
    {
        //cria Cartheader e CartItems
        _context.CartHeaders.Add(cart.CartHeader);
        await _context.SaveChangesAsync();

        cart.CartItems.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
        cart.CartItems.FirstOrDefault().Product = null;

        _context.CartItems.Add(cart.CartItems.FirstOrDefault());

        await _context.SaveChangesAsync();
    }

    private async Task SaveProductInDataBase(CartDTO cartDto, Cart cart)
    {
       //verifica se o produto ja foi salvo senao salva
       var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cart.CartItems.FirstOrDefault().ProductId);

        if (product is null)
        {
            _context.Products.Add(cart.CartItems.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        };
    }

    public Task<bool> ApplyCouponsAsync(string userId, string couponCode)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCouponsAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
