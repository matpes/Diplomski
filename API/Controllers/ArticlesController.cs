using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ArticlesController : BaseApiController
    {
        private readonly IArticlesRepository _articlesRepository;
        public ArticlesController(IArticlesRepository articlesRepository)
        {
            _articlesRepository = articlesRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
        {
            return Ok(await _articlesRepository.getArticlesAsync());
        }
    }
}