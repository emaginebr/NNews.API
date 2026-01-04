using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NAuth.ACL.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;

namespace NNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserClient _userClient;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IUserClient userClient, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IList<CategoryInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                var categories = _categoryService.ListAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing categories");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error listing categories" });
            }
        }

        /// <summary>
        /// Lists categories filtered by roles and parent ID
        /// </summary>
        /// <param name="roles">List of role slugs (comma-separated)</param>
        /// <param name="parentId">Parent category ID (optional)</param>
        /// <returns>List of filtered categories with published articles</returns>
        [HttpGet("listByParent")]
        [ProducesResponseType(typeof(IList<CategoryInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetByRolesAndParent([FromQuery] string? roles, [FromQuery] long? parentId)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);

                var categories = _categoryService.ListByParent(userSession?.Roles, parentId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering categories by roles and parent");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error filtering categories" });
            }
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var category = _categoryService.GetById(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Category not found: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category: {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching category" });
            }
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">Category data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CategoryInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Insert([FromBody] CategoryInfo category)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdCategory = _categoryService.Insert(category);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when creating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating category" });
            }
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="category">Updated category data</param>
        /// <returns>Updated category</returns>
        [HttpPut()]
        [Authorize]
        [ProducesResponseType(typeof(CategoryInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update([FromBody] CategoryInfo category)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (category.CategoryId <= 0)
                    return BadRequest(new { message = "Category ID is empty" });

                var updatedCategory = _categoryService.Update(category);
                return Ok(updatedCategory);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Category not found for update: {CategoryId}", category.CategoryId);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when updating category: {CategoryId}", category.CategoryId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {CategoryId}", category.CategoryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating category" });
            }
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Operation status</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                _categoryService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Category not found for deletion: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting category" });
            }
        }
    }
}
