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
        public OrderController(OnlineStoreContext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
                // Create the Order entity
                var order = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    Status = orderDto.Status,
                    CustomerId = orderDto.CustomerId
                };

                order.OrderDetails = new List<OrderDetail>();

                // Add the OrderDetails to the order
                foreach (var detailDto in orderDto.OrderDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = detailDto.ProductId,
                        Quantity = detailDto.Quantity,
                        PriceAtPurchase = detailDto.PriceAtPurchase
                    };

                    order.OrderDetails.Add(orderDetail);
                }

                // Add the order to the database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();


                // Return a response with the created order to ensure that the order is placed in the database correctly 
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }

            // If the model is invalid, return a bad request response with validation errors
            return BadRequest(ModelState);
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
