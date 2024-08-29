using System.ComponentModel.DataAnnotations;

namespace OnlineStoreAPI.Models
{
    public class CustomerDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
