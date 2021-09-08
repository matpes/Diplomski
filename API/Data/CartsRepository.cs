using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class CartsRepository : ICartsRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CartsRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async void addToCart(CartDto cartDto)
        {
            Cart cart = new Cart{};
            cart.bought = cartDto.bought;
            cart.ArticleId = cartDto.Article.Id;
            cart.AppUserId = cartDto.AppUser.Id;
            cart.kolicina = cartDto.kolicina;
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async void buyAllFromCarts(CartDto[] carts)
        {
            foreach(var cart in carts){
                int id = cart.Id;
                var dbCart = await _context.Carts.FindAsync(id);
                dbCart.bought = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartDto>> getCartContentForUserId(int id)
        {
            var carts =  await _context.Carts.Where(x => x.AppUserId == id).Where(x => x.bought == false).Include(x => x.Article).ProjectTo<CartDto>(_mapper.ConfigurationProvider).ToListAsync();
            return carts;
        }

        public async void removeArticleFromCart(int id)
        {
            Cart cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}