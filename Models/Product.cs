using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name{ get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int QuantityInStock { get; set; }

        // Navigation property for the junction table
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}