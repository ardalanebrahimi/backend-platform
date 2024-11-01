// IImageUploadService.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ardiland.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImage(IFormFile imageFile);
    }
}
