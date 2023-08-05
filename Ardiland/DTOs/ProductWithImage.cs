// ProductWithImage.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTOs
{
    public class ProductWithImage
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Please select an image.")]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
