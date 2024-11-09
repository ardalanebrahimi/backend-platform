using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Ardiland.Models
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("passwordhash")]
        public string PasswordHash { get; set; }
    }
}
