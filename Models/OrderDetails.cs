using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }

        /// Foreign keys 
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public virtual Order Order { get; set; } ///
        public virtual Product Product { get; set; }
    }
}
