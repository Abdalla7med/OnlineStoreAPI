using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        OnlineStoreContext _context;

        public CustomerController(OnlineStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var Customers = await _context.Customers.ToListAsync();

            return Ok(Customers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomers([FromBody] CustomerCreateDTO CustomerDto)
        {
            if (_context.Customers.Any(c => c.Name == CustomerDto.Name))
                return BadRequest("Customer Exists try to Update it");

            if (ModelState.IsValid)
            {
                var customer = new Customer()
                {
                    Name = CustomerDto.Name,
                    Email = CustomerDto.Email,
                    PhoneNumber = CustomerDto.PhoneNumber
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProductById), new { id = customer.Id }, customer);
            }

            return BadRequest(ModelState);
        }
     
        [HttpPut("{id}")]
        public async Task<IActionResult> Updateustomer(int id, [FromBody] CustomerUpdateDTO customerDto)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Try to Add Customer First "); // Return 404 if the customer is not found
            }

            customer.Email = customerDto.Email;
            customer.PhoneNumber = customerDto.PhoneNumber;


            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content status code

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var customer = await _context.Customers
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Product not Found");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
