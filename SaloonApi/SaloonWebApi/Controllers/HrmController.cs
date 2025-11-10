using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/hrm")]
    public class HrmController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public HrmController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet("employees/service-persons")]
        public async Task<IActionResult> GetServicePersons()
        {
            var list = await _db.Employees.Where(e => e.Active == 1).ToListAsync();
            return Ok(list);
        }
    }
}