using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StoresController : BaseApiController
    {
        private readonly IStoresRepository _storesRepository;
        public StoresController(IStoresRepository storesRepository)
        {
            _storesRepository = storesRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreDto>>> GetShops()
        {
            try
            {
                return Ok(await _storesRepository.getStoreDetails());
            }
            catch (Exception)
            {
                return BadRequest("Something failed");
            }
        }

        [HttpPost("details/crawl")]
        public ActionResult getCartContentForUserId(StoreDto storeDto)
        {
            _storesRepository.setCrawlingDetails(storeDto);
            return NoContent();
        }

        [HttpGet("delete/{id}")]
        public ActionResult deleteArticlesFromStore(int id)
        {
            _storesRepository.deleteArticlesFromStore(id);
            return NoContent();
        }

    }
}