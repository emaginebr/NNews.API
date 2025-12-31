using Microsoft.AspNetCore.Http;

namespace NNews.ACL.Interfaces
{
    public interface IImageClient
    {
        Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
