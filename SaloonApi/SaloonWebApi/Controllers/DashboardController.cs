using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public DashboardController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary()
        {
            var total = await _db.BookingMasters.CountAsync();
            var completed = await _db.BookingMasters.CountAsync(b => b.Booking_Status_Id == 2);
            var pending = await _db.BookingMasters.CountAsync(b => b.Booking_Status_Id == 1);
            return Ok(new { total, completed, pending });
        }
    }
}