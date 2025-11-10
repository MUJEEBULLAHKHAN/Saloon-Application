using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductsController(SaloonDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Products.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (dto == null) return BadRequest();
            var p = new Product
            {
                Product_Code = dto.Product_Code,
                Product_Name = dto.Product_Name,
                Prod_Name = dto.Prod_Name,
                Product_Description = dto.Product_Description,
                Brand = dto.Brand,
                Weight = dto.Weight,
                Unit = dto.Unit,
                Category = dto.Category,
                Unit_Price = dto.Unit_Price,
                Company_Price = dto.Company_Price,
                Min_Price = dto.Min_Price,
                Max_Price = dto.Max_Price,
                Percentage = dto.Percentage,
                Sale_Price = dto.Sale_Price,
                Product_Image = dto.Product_Image,
                Account_No = dto.Account_No,
                No_of_Times = dto.No_of_Times,
                Barcode = dto.Barcode,
                Active = 1,
                Insert_User = 0,
                Insert_Date = DateTime.UtcNow
            };

            // if caller provided a base64 image, save it to product_images and set Product_Image to the saved relative path
            if (!string.IsNullOrWhiteSpace(dto.Product_ImageBase64))
            {
                try
                {
                    var base64 = dto.Product_ImageBase64!;
                    var markerIndex = base64.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
                    if (markerIndex >= 0)
                        base64 = base64.Substring(markerIndex + 7);

                    var imageBytes = Convert.FromBase64String(base64);

                    // determine extension from data URI if present
                    string ext = "png";
                    if (dto.Product_ImageBase64!.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    {
                        var mimePart = dto.Product_ImageBase64.Substring(5);
                        var semi = mimePart.IndexOf(';');
                        if (semi > 0)
                        {
                            var mime = mimePart.Substring(0, semi); // e.g. image/png
                            if (mime.StartsWith("image/"))
                            {
                                ext = mime.Substring("image/".Length).Replace("jpeg", "jpg");
                            }
                        }
                    }

                    var fileName = $"{Guid.NewGuid():N}.{ext}";
                    var webRoot = _env.WebRootPath;
                    var basePath = !string.IsNullOrEmpty(webRoot) ? webRoot : _env.ContentRootPath;
                    var imagesFolder = Path.Combine(basePath, "product_images");
                    Directory.CreateDirectory(imagesFolder);
                    var physicalPath = Path.Combine(imagesFolder, fileName);
                    await System.IO.File.WriteAllBytesAsync(physicalPath, imageBytes);

                    // store relative path
                    p.Product_Image = Path.Combine("product_images", fileName).Replace('\\', '/');
                }
                catch
                {
                    // ignore and continue without image if invalid base64
                }
            }

            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = p.Product_Id }, p);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product model)
        {
            if (model == null || id != model.Product_Id) return BadRequest();
            var existing = await _db.Products.FindAsync(id);
            if (existing == null) return NotFound();
            // simple update
            existing.Product_Code = model.Product_Code;
            existing.Product_Name = model.Product_Name;
            existing.Prod_Name = model.Prod_Name;
            existing.Product_Description = model.Product_Description;
            existing.Brand = model.Brand;
            existing.Weight = model.Weight;
            existing.Unit = model.Unit;
            existing.Category = model.Category;
            existing.Unit_Price = model.Unit_Price;
            existing.Company_Price = model.Company_Price;
            existing.Min_Price = model.Min_Price;
            existing.Max_Price = model.Max_Price;
            existing.Percentage = model.Percentage;
            existing.Sale_Price = model.Sale_Price;
            existing.Product_Image = model.Product_Image;
            existing.Account_No = model.Account_No;
            existing.No_of_Times = model.No_of_Times;
            existing.Barcode = model.Barcode;
            existing.Active = model.Active;
            existing.Modify_User = model.Modify_User;
            existing.Modify_Date = DateTime.UtcNow;
            _db.Products.Update(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing == null) return NotFound();
            _db.Products.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
