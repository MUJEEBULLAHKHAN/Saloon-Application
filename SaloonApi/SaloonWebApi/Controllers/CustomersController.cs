using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public CustomersController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _db.Customers.Select(c => new CustomerDto
            {
                Customer_Id = c.Customer_Id,
                Customer_Code = c.Customer_Code,
                Customer_Name = c.Customer_Name,
                Mobile_No = c.Mobile_No,
                Email = c.Email
            }).ToListAsync();

            return Ok(customers);
        }

        [HttpGet("by-mobile/{mobile}")]
        public async Task<IActionResult> GetByMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return BadRequest("mobile is required.");

            // Normalize input: remove non-digits, handle common country-code patterns
            static string NormalizeInput(string input)
            {
                input = input.Trim();
                // keep digits only
                var digitsOnly = System.Text.RegularExpressions.Regex.Replace(input, @"\D", "");

                if (string.IsNullOrEmpty(digitsOnly)) return input;

                // common conversions for Pakistan (+92)
                if (input.StartsWith("+"))
                    return "+" + digitsOnly;
                if (digitsOnly.StartsWith("00") && digitsOnly.Length > 2)
                    return "+" + digitsOnly.Substring(2);
                if (digitsOnly.StartsWith("92"))
                    return "+" + digitsOnly;
                if (digitsOnly.StartsWith("0"))
                    return "+92" + digitsOnly.Substring(1);

                // fallback: return digits and also the +digits form may be useful
                return digitsOnly;
            }

            var normalized = NormalizeInput(mobile);
            var digits = System.Text.RegularExpressions.Regex.Replace(mobile, @"\D", "");

            // build candidate matches that exist in DB (cover typical stored formats)
            var candidates = new List<string>();
            if (!string.IsNullOrEmpty(normalized)) candidates.Add(normalized);
            if (!string.IsNullOrEmpty(digits)) candidates.Add(digits);
            if (!string.IsNullOrEmpty(digits) && !digits.StartsWith("0")) candidates.Add("0" + digits);
            if (!string.IsNullOrEmpty(digits) && !digits.StartsWith("92")) candidates.Add("92" + digits);
            if (!string.IsNullOrEmpty(digits) && !digits.StartsWith("+")) candidates.Add("+" + digits);

            // remove duplicates
            candidates = candidates.Distinct().ToList();

            // query DB for any matching stored format
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => candidates.Contains(c.Mobile_No));

            if (customer == null) return NotFound();
            return Ok(new CustomerDto
            {
                Customer_Id = customer.Customer_Id,
                Customer_Code = customer.Customer_Code,
                Customer_Name = customer.Customer_Name,
                Mobile_No = customer.Mobile_No,
                Email = customer.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            var exists = await _db.Customers.AnyAsync(c => c.Mobile_No == dto.Mobile_No);
            if (exists) return Conflict("Customer with same mobile already exists.");

            var customer = new Customer
            {
                Customer_Code = dto.Customer_Code,
                Customer_Name = dto.Customer_Name,
                Mobile_No = dto.Mobile_No,
                Email = dto.Email,
                Registration_Date = DateTime.UtcNow,
                Active = 1,
                Insert_User = 0,
                Insert_Date = DateTime.UtcNow
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByMobile), new { mobile = customer.Mobile_No }, new CustomerDto
            {
                Customer_Id = customer.Customer_Id,
                Customer_Code = customer.Customer_Code,
                Customer_Name = customer.Customer_Name,
                Mobile_No = customer.Mobile_No,
                Email = customer.Email
            });
        }
    }
}