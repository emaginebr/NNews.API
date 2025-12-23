using Microsoft.AspNetCore.Mvc;
using NNews.Domain.Services.Interfaces;
using NNews.Dtos;

namespace NNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(IArticleService articleService, ILogger<ArticleController> logger)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists all articles
        /// </summary>
        /// <param name="categoryId">Optional category ID to filter articles</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of articles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll([FromQuery] long? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var articles = _articleService.ListAll(categoryId, page, pageSize);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing articles");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error listing articles" });
            }
        }

        /// <summary>
        /// Filters published articles by roles and parent category ID
        /// </summary>
        /// <param name="roles">List of role slugs (comma-separated, optional)</param>
        /// <param name="parentId">Parent category ID (optional)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of filtered published articles</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Filter([FromQuery] string? roles, [FromQuery] long? parentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                IList<string>? rolesList = null;
                if (!string.IsNullOrWhiteSpace(roles))
                {
                    rolesList = roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim())
                        .ToList();
                }

                var articles = _articleService.FilterByRolesAndParent(rolesList, parentId, page, pageSize);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering articles by roles and parent");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error filtering articles" });
            }
        }

        /// <summary>
        /// Gets an article by ID
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <returns>Article found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var article = _articleService.GetById(id);
                return Ok(article);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Article not found: {ArticleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching article: {ArticleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching article" });
            }
        }

        /// <summary>
        /// Creates a new article
        /// </summary>
        /// <param name="article">Article data</param>
        /// <returns>Created article</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] ArticleInfo article)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdArticle = _articleService.Insert(article);
                return CreatedAtAction(nameof(GetById), new { id = createdArticle.ArticleId }, createdArticle);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when creating article");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating article" });
            }
        }

        /// <summary>
        /// Updates an existing article
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <param name="article">Updated article data</param>
        /// <returns>Updated article</returns>
        [HttpPut]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update([FromBody] ArticleInfo article)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (article.ArticleId <= 0)
                    return BadRequest(new { message = "Article ID cant be empty" });

                var updatedArticle = _articleService.Update(article);
                return Ok(updatedArticle);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Article not found for update: {ArticleId}", article.ArticleId);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when updating article: {ArticleId}", article.ArticleId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article: {ArticleId}", article.ArticleId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating article" });
            }
        }
    }
}
