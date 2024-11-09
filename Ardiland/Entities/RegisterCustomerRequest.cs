using System.ComponentModel.DataAnnotations.Schema;

namespace Ardiland.Entities
{
    public class RegisterCustomerRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
