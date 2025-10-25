using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Route("api/lookups")]
    public class LookupsController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public LookupsController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _db.Services.Select(s => new ServiceDto
            {
                Service_Id = s.Service_Id,
                Service_Code = s.Service_Code,
                Service_Name = s.Service_Name,
                Service_Price = s.Service_Price,
                Service_Category = s.Service_Category
            }).ToListAsync();

            return Ok(services);
        }

        [HttpPost("services")]
        public async Task<IActionResult> CreateService(ServiceDto dto)
        {
            var s = new Service
            {
                Service_Code = dto.Service_Code,
                Service_Name = dto.Service_Name,
                Service_Description = string.Empty,
                Service_Price = dto.Service_Price,
                Service_Category = dto.Service_Category,
                Account_No = string.Empty,
                Active = 1,
                Insert_User = 0,
                Insert_Date = DateTime.UtcNow
            };
            _db.Services.Add(s);
            await _db.SaveChangesAsync();
            dto.Service_Id = s.Service_Id;
            return CreatedAtAction(nameof(GetServices), new { id = s.Service_Id }, dto);
        }

        [HttpPut("services/{id}")]
        public async Task<IActionResult> UpdateService(int id, ServiceDto dto)
        {
            var s = await _db.Services.FindAsync(id);
            if (s == null) return NotFound();
            s.Service_Name = dto.Service_Name;
            s.Service_Price = dto.Service_Price;
            s.Service_Code = dto.Service_Code;
            s.Service_Category = dto.Service_Category;
            s.Modify_Date = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("services/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var s = await _db.Services.FindAsync(id);
            if (s == null) return NotFound();
            _db.Services.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("deals")]
        public async Task<IActionResult> GetDeals()
        {
            var deals = await _db.Deals.Select(d => new DealDto
            {
                Deal_Id = d.Deal_Id,
                Deal_Name = d.Deal_Name,
                Deal_Price = d.Deal_Price,
                Active = d.Active
            }).ToListAsync();
            return Ok(deals);
        }

        [HttpPost("deals")]
        public async Task<IActionResult> CreateDeal(DealDto dto)
        {
            var d = new Deal
            {
                Deal_Name = dto.Deal_Name,
                Deal_Price = dto.Deal_Price,
                Active = dto.Active,
                Insert_Date = DateTime.UtcNow,
                Insert_User = 0
            };
            _db.Deals.Add(d);
            await _db.SaveChangesAsync();
            dto.Deal_Id = d.Deal_Id;
            return CreatedAtAction(nameof(GetDeals), new { id = d.Deal_Id }, dto);
        }

        [HttpPut("deals/{id}")]
        public async Task<IActionResult> UpdateDeal(int id, DealDto dto)
        {
            var d = await _db.Deals.FindAsync(id);
            if (d == null) return NotFound();
            d.Deal_Name = dto.Deal_Name;
            d.Deal_Price = dto.Deal_Price;
            d.Active = dto.Active;
            d.Update_Date = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("deals/{id}")]
        public async Task<IActionResult> DeleteDeal(int id)
        {
            var d = await _db.Deals.FindAsync(id);
            if (d == null) return NotFound();
            _db.Deals.Remove(d);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("bookingstatus")]
        public async Task<IActionResult> GetBookingStatus()
        {
            var list = await _db.BookingStatuses.ToListAsync();
            return Ok(list);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("servicestatus")]
        public async Task<IActionResult> GetServiceStatus()
        {
            var list = await _db.BookingServiceStatuses.ToListAsync();
            return Ok(list);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("paymentstatus")]
        public IActionResult GetPaymentStatus()
        {
            // static for now
            var vals = new[] { "Pending", "Paid", "Partial" };
            return Ok(vals);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _db.Roles.ToListAsync();
            return Ok(roles);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("activitylogs")]
        public async Task<IActionResult> GetActivityLogs()
        {
            // Not implemented - return empty
            return Ok(new object[0]);
        }
    }
}