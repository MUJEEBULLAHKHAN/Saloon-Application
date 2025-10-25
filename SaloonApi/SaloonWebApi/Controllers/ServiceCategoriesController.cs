using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceCategoriesController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public ServiceCategoriesController(SaloonDbContext db) => _db = db;

        // GET: api/servicecategories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.ServiceCategories
                .AsNoTracking()
                .Select(sc => new ServiceCategoryDto
                {
                    Category_Id = sc.Category_Id,
                    Category_Name = sc.Category_Name,
                    Category_Description = sc.Category_Description,
                    Category_Image = sc.Category_Image,
                    Active = sc.Active,
                    Insert_User = sc.Insert_User,
                    Insert_Date = sc.Insert_Date,
                    Modify_User = sc.Modify_User,
                    Modify_Date = sc.Modify_Date
                })
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/servicecategories/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sc = await _db.ServiceCategories.FindAsync(id);
            if (sc == null) return NotFound();
            var dto = new ServiceCategoryDto
            {
                Category_Id = sc.Category_Id,
                Category_Name = sc.Category_Name,
                Category_Description = sc.Category_Description,
                Category_Image = sc.Category_Image,
                Active = sc.Active,
                Insert_User = sc.Insert_User,
                Insert_Date = sc.Insert_Date,
                Modify_User = sc.Modify_User,
                Modify_Date = sc.Modify_Date
            };
            return Ok(dto);
        }

        // POST: api/servicecategories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceCategoryDto dto)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Category_Name)) return BadRequest("Category_Name is required.");

            var entity = new ServiceCategory
            {       
                Category_Name = dto.Category_Name,
                Category_Description = dto.Category_Description,
                Category_Image = dto.Category_Image,
                Active = dto.Active,
                Insert_User = dto.Insert_User,
                Insert_Date = DateTime.UtcNow
            };

            _db.ServiceCategories.Add(entity);
            await _db.SaveChangesAsync();

            var result = new ServiceCategoryDto
            {
                Category_Id = entity.Category_Id,
                Category_Name = entity.Category_Name,
                Category_Description = entity.Category_Description,
                Category_Image = entity.Category_Image,
                Active = entity.Active,
                Insert_User = entity.Insert_User,
                Insert_Date = entity.Insert_Date
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Category_Id }, result);
        }

        // PUT: api/servicecategories/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceCategoryDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _db.ServiceCategories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Category_Name = dto.Category_Name;
            existing.Category_Description = dto.Category_Description;
            existing.Category_Image = dto.Category_Image;
            existing.Active = dto.Active;
            existing.Modify_User = dto.Insert_User;
            existing.Modify_Date = DateTime.UtcNow;

            _db.ServiceCategories.Update(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/servicecategories/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.ServiceCategories.FindAsync(id);
            if (existing == null) return NotFound();

            _db.ServiceCategories.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}