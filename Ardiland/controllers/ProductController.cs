using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ardiland.Data;
using Ardiland.Models;
using Ardiland.Services;
using Microsoft.AspNetCore.Authorization;

namespace Ardiland.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IImageUploadService _imageUploadService;


        public ProductsController(AppDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();

            return Ok(products);
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/Products
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductWithImage productWithImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Convert the list of image URLs to a JSON string
            //var imagesJson = JsonSerializer.Serialize(new List<string> { imageUrl });

            // Create the product with the uploaded image URL
            var product = new Product
            {
                Name = productWithImage.Name,
                Description = productWithImage.Description,
                Price = productWithImage.Price,
            };
            // Handle the image upload
            foreach (IFormFile imageFile in productWithImage.ImageFiles)
            {
                var imageUrl = await _imageUploadService.UploadImage(imageFile);
                if (product.Images == null) product.Images = new List<string>();
                product.Images.Add(imageUrl);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }


        // PUT: api/products/{id}
        [Authorize]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductWithImage updatedProduct)
        {
            var existingProduct = _context.Products.Find(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update simple properties of the existing product
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;

            // Handle updating images
            if (updatedProduct.ImageFiles != null)
            {
                foreach (IFormFile imageFile in updatedProduct.ImageFiles)
                {
                    var imageUrl = await _imageUploadService.UploadImage(imageFile);
                    if (existingProduct.Images == null) existingProduct.Images = new List<string>();
                    existingProduct.Images.Add(imageUrl);
                }
                _context.Products.Update(existingProduct);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/Products/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/products/{id}/images
        [Authorize]
        [HttpDelete("{id}/images")]
        public async Task<IActionResult> DeleteProductImage(Guid id, [FromQuery] string imageUrl)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Remove the image URL from the product's images list
            existingProduct.Images.Remove(imageUrl);

            // Mark the entity as modified
            _context.Products.Update(existingProduct);

            // TODO: Implement the logic to delete the image file from Azure Blob Storage

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
