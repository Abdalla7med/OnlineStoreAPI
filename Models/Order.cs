namespace Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
       
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; }

        public Customer Customer { get; set; }

        /// Navigational Property for Many To Many Relation
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
