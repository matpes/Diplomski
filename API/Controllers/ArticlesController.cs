using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Extensions;
using API.Helpers;
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
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles([FromQuery]ArticlesParams articleParams)
        {
            var articles = await _articlesRepository.getArticlesAsync(articleParams);
            Response.AddPaginationHeader(articles.CurrentPage, articles.PageSize, articles.TotalCount, articles.TotalPages);
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticleById(int id){
            return Ok(await _articlesRepository.getArticleByIdAsync(id));
        }
    }
}