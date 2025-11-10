using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.Models;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DealCategoriesController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public DealCategoriesController(SaloonDbContext db) => _db = db;

        // GET: api/dealcategories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.DealCategories.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        // GET: api/dealcategories/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.DealCategories.FindAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // POST: api/dealcategories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DealCategory model)
        {
            if (model == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(model.Deal_Category_Name)) return BadRequest("Deal_Category_Name is required.");

            model.Insert_Date = DateTime.UtcNow;
            model.Insert_User = model.Insert_User; // keep provided or default caller-supplied
            _db.DealCategories.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // PUT: api/dealcategories/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DealCategory model)
        {
            if (model == null) return BadRequest();
            if (id != model.Id) return BadRequest("Id mismatch.");

            var existing = await _db.DealCategories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Deal_Category_Name = model.Deal_Category_Name;
            existing.Active = model.Active;
            existing.Modify = model.Modify;
            existing.Modify_User = model.Modify_User;
            existing.Modify_Date = DateTime.UtcNow;

            _db.DealCategories.Update(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/dealcategories/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.DealCategories.FindAsync(id);
            if (existing == null) return NotFound();

            _db.DealCategories.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}