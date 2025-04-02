using Microsoft.AspNetCore.Mvc;
using BankBackOffice.Data;
using BankBackOffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BankBackOffice.Controllers
{
    //[Authorize]

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAllCustomers() => Ok(_context.Customers.ToList());


        [Authorize]
        [HttpPost]
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Customers.Any(c => c.CustomerNumber == customer.CustomerNumber))
                return BadRequest("Customer number already exists.");

            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Ok(customer);
        }



        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
        {
            var existing = _context.Customers.Find(id);
            if (existing == null) return NotFound();

            existing.CustomerName = customer.CustomerName;
            existing.DateOfBirth = customer.DateOfBirth;
            existing.Gender = customer.Gender;
            _context.SaveChanges();
            return Ok(existing);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return NotFound("Customer not found.");
            return Ok(customer);
        }

        [Authorize]
        [HttpGet("search/number/{customerNumber}")]
        public async Task<ActionResult<Customer>> GetByCustomerNumber(int customerNumber)
        {
            var customer = await _context.Customers.FindAsync(customerNumber);

            if (customer == null)
                return NotFound("Customer number not valid.");

            return Ok(customer);
        }

        [Authorize]
        [HttpGet("search/name/{customerName}")]
        public async Task<ActionResult<List<Customer>>> GetByCustomerName(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return BadRequest("Customer name cannot be empty.");

            var customers = await _context.Customers
                .Where(c => c.CustomerName.ToLower().Contains(customerName.ToLower()))
                .ToListAsync();

            if (!customers.Any())
                return NotFound("No customers found with that name.");

            return Ok(customers);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return NotFound("Customer not found.");

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            
            return Ok($"Customer {id} deleted.");
        }


    }
}
