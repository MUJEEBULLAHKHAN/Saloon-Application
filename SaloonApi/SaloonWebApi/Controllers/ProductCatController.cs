using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.Models;
using SaloonWebApi.DTOs;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductCatController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public ProductCatController(SaloonDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.ProductCats.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.ProductCats.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCat model)
        {
            if (model == null) return BadRequest();
            model.Insert_Date = DateTime.UtcNow;
            _db.ProductCats.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = model.Category_Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCat model)
        {
            if (model == null || id != model.Category_Id) return BadRequest();
            var existing = await _db.ProductCats.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Category_Code = model.Category_Code;
            existing.Category_Name = model.Category_Name;
            existing.Category_Description = model.Category_Description;
            existing.Active = model.Active;
            existing.Modify_User = model.Modify_User;
            existing.Modify_Date = DateTime.UtcNow;
            _db.ProductCats.Update(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.ProductCats.FindAsync(id);
            if (existing == null) return NotFound();
            _db.ProductCats.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
