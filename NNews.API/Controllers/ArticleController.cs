using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NAuth.ACL.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;
using NNews.DTO.AI;

namespace NNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleAIService _articleAIService;
        private readonly IUserClient _userClient;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
            IArticleService articleService,
            IArticleAIService articleAIService,
            IUserClient userClient,
            ILogger<ArticleController> logger)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _articleAIService = articleAIService ?? throw new ArgumentNullException(nameof(articleAIService));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
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
        [Authorize]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll([FromQuery] long? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

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
        /// <param name="categoryId">Category ID (required)</param>
        /// <param name="roles">List of role slugs (comma-separated, optional)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of filtered published articles</returns>
        [HttpGet("ListByCategory")]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListByCategory([FromQuery] long categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (categoryId <= 0)
                    return BadRequest(new { message = "Category ID is required and must be greater than zero." });

                var userSession = _userClient.GetUserInSession(HttpContext);

                var articles = _articleService.ListByCategory(userSession?.Roles, categoryId, page, pageSize);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering articles by category and roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error filtering articles" });
            }
        }

        /// <summary>
        /// Lists published articles filtered by roles
        /// </summary>
        /// <param name="roles">List of role slugs (comma-separated, optional)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of published articles filtered by roles</returns>
        [HttpGet("ListByRoles")]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListByRoles([FromQuery] string? roles, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);

                var articles = _articleService.ListByRoles(userSession?.Roles, page, pageSize);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering articles by roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error filtering articles by roles" });
            }
        }

        /// <summary>
        /// Lists published articles filtered by tag slug and optionally by roles
        /// </summary>
        /// <param name="tagSlug">Tag slug (required)</param>
        /// <param name="roles">List of role slugs (comma-separated, optional)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of published articles filtered by tag and roles</returns>
        [HttpGet("ListByTag")]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListByTag([FromQuery] string tagSlug, [FromQuery] string? roles, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tagSlug))
                    return BadRequest(new { message = "Tag slug is required." });

                var userSession = _userClient.GetUserInSession(HttpContext);

                var articles = _articleService.ListByTag(userSession?.Roles, tagSlug, page, pageSize);
                return Ok(articles);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameters for filtering articles by tag");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering articles by tag and roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error filtering articles by tag" });
            }
        }

        /// <summary>
        /// Searches published articles by keyword in title and content, optionally filtered by roles
        /// </summary>
        /// <param name="keyword">Search keyword (required)</param>
        /// <param name="roles">List of role slugs (comma-separated, optional)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of published articles matching the search criteria</returns>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(PagedResult<ArticleInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Search([FromQuery] string keyword, [FromQuery] string? roles, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return BadRequest(new { message = "Search keyword is required." });

                var userSession = _userClient.GetUserInSession(HttpContext);

                var articles = _articleService.Search(userSession?.Roles, keyword, page, pageSize);
                return Ok(articles);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameters for searching articles");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching articles by keyword");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error searching articles" });
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
        [Authorize]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Insert([FromBody] ArticleInsertedInfo article)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

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
        /// Creates a new article using AI
        /// </summary>
        /// <param name="request">AI article creation request with prompt and options</param>
        /// <returns>Created article</returns>
        [HttpPost("insertWithAI")]
        [Authorize]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertWithAI([FromBody] AIArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(request.Prompt))
                    return BadRequest(new { message = "Prompt is required." });

                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                var createdArticle = await _articleAIService.InsertWithAI(request.Prompt, request.GenerateImage);
                return CreatedAtAction(nameof(GetById), new { id = createdArticle.ArticleId }, createdArticle);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when creating article with AI");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error with AI response when creating article");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article with AI");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating article with AI" });
            }
        }

        /// <summary>
        /// Updates an existing article
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <param name="article">Updated article data</param>
        /// <returns>Updated article</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update([FromBody] ArticleUpdatedInfo article)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

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

        /// <summary>
        /// Updates an existing article using AI
        /// </summary>
        /// <param name="request">AI article update request with articleId, prompt and options</param>
        /// <returns>Updated article</returns>
        [HttpPut("updateWithAI")]
        [Authorize]
        [ProducesResponseType(typeof(ArticleInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateWithAI([FromBody] AIArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!request.ArticleId.HasValue || request.ArticleId.Value <= 0)
                    return BadRequest(new { message = "ArticleId is required and must be greater than zero." });

                if (string.IsNullOrWhiteSpace(request.Prompt))
                    return BadRequest(new { message = "Prompt is required." });

                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                var updatedArticle = await _articleAIService.UpdateWithAI((int)request.ArticleId.Value, request.Prompt, request.GenerateImage);
                return Ok(updatedArticle);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Article not found when updating with AI: {ArticleId}", request.ArticleId);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when updating article with AI: {ArticleId}", request.ArticleId);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error with AI response when updating article: {ArticleId}", request.ArticleId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article with AI: {ArticleId}", request.ArticleId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating article with AI" });
            }
        }
    }
}
