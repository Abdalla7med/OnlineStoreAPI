using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        OnlineStoreContext _context;
        ILogger<OrderController> logger;
        public OrderController(OnlineStoreContext context, ILogger<OrderController> logger)
        {
            _context = context;
            logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult>  GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();

            return Ok(orders);  
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDto)
        {
            // Check if the incoming order data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == orderDto.CustomerId);

            // Ensure that the customer who places the order exists
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Initialize a list to collect any errors related to stock availability
            var stockErrors = new List<string>();

            // Create the Order entity
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = orderDto.Status,
                CustomerId = orderDto.CustomerId,
                Customer = customer
            };

            order.OrderDetails = new List<OrderDetail>();

            // Check if all products in the order have sufficient stock
            foreach (var detailDto in orderDto.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == detailDto.ProductId);

                if (product == null)
                {
                    stockErrors.Add($"Product with ID {detailDto.ProductId} not found.");
                    continue;
                }

                if (detailDto.Quantity > product.QuantityInStock)
                {

                    stockErrors.Add($"Insufficient stock for Product ID {detailDto.ProductId}. Available: {product.QuantityInStock}, Requested: {detailDto.Quantity}");
                    continue;
                }

                // If stock is sufficient, add to the order details
                var orderDetail = new OrderDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    PriceAtPurchase = product.Price * detailDto.Quantity
                };
                order.OrderDetails.Add(orderDetail);
            }

            // Return errors if any stock issues were found
            if (stockErrors.Any())
            {
                return BadRequest(string.Join("; ", stockErrors));
            }

            // Update stock quantities for valid order details
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);
                if (product != null)
                {
                    product.QuantityInStock -= orderDetail.Quantity;
                    _context.Update(product);
                }
            }

            // Add the order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return a response with the created order
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders
                                        .Include(o => o.OrderDetails)
                                        .ThenInclude(od => od.Product)
                                        .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDTO OrderDto)
        {
            // Check if the order exists
            var order = await _context.Orders
                                      .Include(o => o.OrderDetails) // Include OrderDetails if needed for any validation or logic
                                      .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Try to add order first"); // Return 404 if the order is not found
            }

            // Update only the Status property
            order.Status = OrderDto.Status;

            // Mark the entity as modified
            _context.Entry(order).State = EntityState.Modified;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content status code

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var Order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (Order == null)
                return NotFound("Order Doesn't exists");

            _context.Orders.Remove(Order);
            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
}
