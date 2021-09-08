using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces
{
    public interface ICartsRepository
    {
        void addToCart(CartDto cartDto);

        Task<IEnumerable<CartDto>> getCartContentForUserId(int id);

        void buyAllFromCarts(CartDto[] carts);

        void removeArticleFromCart(int id);
    }
}