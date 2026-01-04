using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NAuth.ACL.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;

namespace NNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IUserClient _userClient;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, IUserClient userClient, ILogger<TagController> logger)
        {
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lista todas as tags
        /// </summary>
        /// <returns>Lista de tags</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IList<TagInfo>), StatusCodes.Status200OK)]
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
                var tags = _tagService.ListAll();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing tags");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error listing tags" });
            }
        }

        /// <summary>
        /// Lista tags de artigos publicados filtradas por roles
        /// </summary>
        /// <param name="roles">Lista de role slugs (comma-separated, optional)</param>
        /// <returns>Lista de tags filtradas</returns>
        [HttpGet("ListByRoles")]
        [ProducesResponseType(typeof(IList<TagInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ListByRoles()
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);

                var tags = _tagService.ListByRoles(userSession?.Roles);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing tags by roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error listing tags by roles" });
            }
        }

        /// <summary>
        /// Busca uma tag por ID
        /// </summary>
        /// <param name="id">ID da tag</param>
        /// <returns>Tag encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TagInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var tag = _tagService.GetById(id);
                return Ok(tag);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tag not found: {TagId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tag: {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching tag" });
            }
        }

        /// <summary>
        /// Cria uma nova tag
        /// </summary>
        /// <param name="tag">Dados da tag</param>
        /// <returns>Tag criada</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TagInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertAsync([FromBody] TagInfo tag)
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

                var createdTag = await _tagService.InsertAsync(tag);
                return CreatedAtAction(nameof(GetById), new { id = createdTag.TagId }, createdTag);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when creating tag");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma tag existente
        /// </summary>
        /// <param name="tag">Dados atualizados da tag</param>
        /// <returns>Tag atualizada</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(TagInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] TagInfo tag)
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

                var updatedTag = await _tagService.UpdateAsync(tag);
                return Ok(updatedTag);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tag not found for update: {TagId}", tag.TagId);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data when updating tag: {TagId}", tag.TagId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag: {TagId}", tag.TagId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating tag" });
            }
        }

        /// <summary>
        /// Deleta uma tag
        /// </summary>
        /// <param name="id">ID da tag</param>
        /// <returns>Status da operação</returns>
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

                _tagService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tag not found for deletion: {TagId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag: {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting tag" });
            }
        }

        /// <summary>
        /// Mescla uma tag em outra, movendo todos os artigos e excluindo a tag de origem
        /// </summary>
        /// <param name="sourceTagId">ID da tag de origem (será excluída)</param>
        /// <param name="targetTagId">ID da tag de destino (receberá os artigos)</param>
        /// <returns>Status da operação</returns>
        [HttpPost("merge/{sourceTagId:long}/{targetTagId:long}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult MergeTags(long sourceTagId, long targetTagId)
        {
            try
            {
                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                _tagService.MergeTags(sourceTagId, targetTagId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for merging tags: Source={SourceTagId}, Target={TargetTagId}", sourceTagId, targetTagId);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when merging tags: Source={SourceTagId}, Target={TargetTagId}", sourceTagId, targetTagId);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tag not found for merge: Source={SourceTagId}, Target={TargetTagId}", sourceTagId, targetTagId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging tags: Source={SourceTagId}, Target={TargetTagId}", sourceTagId, targetTagId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error merging tags" });
            }
        }
    }
}
