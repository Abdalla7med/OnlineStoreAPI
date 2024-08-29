namespace OnlineStoreAPI
{
    public class OrderDTO
    {
        public string Status { get; set; }
        public int? CustomerId { get; set; } // Optional Foreign Key
        public List<OrderDetailDTO> OrderDetails { get; set; } // List of Order Details

    }
}
