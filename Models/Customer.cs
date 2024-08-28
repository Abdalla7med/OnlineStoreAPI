using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
