using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NAuth.ACL.Interfaces;
using NNews.Domain.Services.Interfaces;
using NTools.ACL.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IUserClient _userClient;
        private readonly IFileClient _imageService;

        public ImageController(
            IUserClient userClient,
            IFileClient imageService
        )
        {
            _userClient = userClient;
            _imageService = imageService;
        }

        [RequestSizeLimit(100_000_000)]
        [HttpPost("uploadImage")]
        [Authorize]
        public async Task<ActionResult<string>> uploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                var userSession = _userClient.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return Unauthorized("Not Authorized");
                }

                var fileName = await _imageService.UploadFileAsync("NNews", file);
                var imageUrl = await _imageService.GetFileUrlAsync("NNews", fileName);
                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
