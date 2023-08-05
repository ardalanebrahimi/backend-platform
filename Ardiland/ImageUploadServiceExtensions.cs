using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YourNamespace.Services;

namespace YourNamespace.Extensions
{
    public static class ImageUploadServiceExtensions
    {
        public static IServiceCollection AddImageUploadService(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the ImageUploadService and pass the IConfiguration to it
            services.AddSingleton<IImageUploadService>(provider => new ImageUploadService(configuration));

            return services;
        }
    }
}
