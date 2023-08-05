using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")] 
        public string Name { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("description")] 
        public string Description { get; set; }


        [Column("images", TypeName = "jsonb")]
        public List<string> Images { get; set; } // Use List<string> for JSON array

    }
}
