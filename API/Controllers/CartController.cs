using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController : BaseApiController
    {
        private readonly ICartsRepository _cartsRepository;
        public CartController(ICartsRepository cartsRepository)
        {
            _cartsRepository = cartsRepository;
        }

        [HttpPost("add")]
        public ActionResult<CartDto> AddToCart(CartDto cartDto)
        {
            try
            {
                _cartsRepository.addToCart(cartDto);
                return Ok(cartDto);
            }
            catch (Exception)
            {
                return BadRequest("Something failed");
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<IEnumerable<CartDto>>> getCartContentForUserId(int id)
        {
            return Ok(await _cartsRepository.getCartContentForUserId(id));
        }

        [HttpGet("remove/{id}")]
        public ActionResult RemoveFromCart(int id)
        {
            _cartsRepository.removeArticleFromCart(id);
            return NoContent();
        }

        [HttpPost("buyAll")]
        public ActionResult BuyAllFromCarts(CartDto[] carts)
        {
            _cartsRepository.buyAllFromCarts(carts);
            return NoContent();
        }

    }
}