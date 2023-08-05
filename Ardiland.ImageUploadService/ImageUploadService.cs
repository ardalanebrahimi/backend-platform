using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace YourNamespace.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IConfiguration _configuration;

        public ImageUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0)
            {
                return null;
            }

            // Generate a unique filename for the image
            var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

            // Get the connection string for your Azure Blob Storage from the appsettings.json file
            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");

            // Get the name of the container where you want to store the images
            var containerName = "images"; // Replace with your container name

            // Create a BlobServiceClient to interact with the Azure Blob Storage
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to the blob where you want to store the image
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload the image file to the blob
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream);
            }

            // Return the URL of the uploaded image
            return blobClient.Uri.ToString();
        }
    }
}
