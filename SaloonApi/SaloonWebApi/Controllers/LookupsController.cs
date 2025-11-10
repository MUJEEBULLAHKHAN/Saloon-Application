using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace SaloonWebApi.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/lookups")]
    public class LookupsController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public LookupsController(SaloonDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _config = config;
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
            var deals = await _db.Deals
                .Select(d => new DealDto
                {
                    Deal_Id = d.Deal_Id,
                    Deal_Name = d.Deal_Name,
                    Deal_Price = d.Deal_Price,
                    Deal_Type = d.Deal_Type,
                    Deal_Cat_Id = d.Deal_Cat_Id,
                    Active = d.Active,
                    Image = d.Image,
                    Insert_User = d.Insert_User,
                    Services = _db.DealServices
                        .Where(ds => ds.Deal_Id == d.Deal_Id)
                        .Select(ds => new DealServiceDto
                        {
                            Service_Id = ds.Service_Id,
                            No_Of_Times = ds.No_Of_Times,
                            Active = ds.Active,
                            Insert_User = ds.Insert_User
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(deals);
        }

        [HttpPost("deals")]
        public async Task<IActionResult> CreateDeal([FromBody] DealDto dto)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Deal_Name)) return BadRequest("Deal_Name is required.");

            // Save image first (if provided)
            string? savedImagePath = null;
            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                try
                {
                    savedImagePath = await SaveBase64ImageAsync(dto.ImageBase64, "image_server");
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid base64 image data.");
                }
            }

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var d = new Deal
                {
                    Deal_Name = dto.Deal_Name,
                    Deal_Price = dto.Deal_Price,
                    Deal_Type = dto.Deal_Type,
                    Deal_Cat_Id = dto.Deal_Cat_Id,
                    Active = dto.Active,
                    Image = savedImagePath ?? dto.Image, // prefer saved path, fallback to dto.Image if provided
                    Insert_Date = DateTime.UtcNow,
                    Insert_User = dto.Insert_User ?? 0
                };

                _db.Deals.Add(d);
                await _db.SaveChangesAsync(); // populate d.Deal_Id

                if (dto.Services != null && dto.Services.Any())
                {
                    var dsList = dto.Services.Select(s => new DealService
                    {
                        Deal_Id = d.Deal_Id,
                        Service_Id = s.Service_Id,
                        No_Of_Times = s.No_Of_Times,
                        Active = s.Active,
                        Insert_User = s.Insert_User ?? (dto.Insert_User ?? 0),
                        Insert_Date = DateTime.UtcNow
                    }).ToList();

                    _db.DealServices.AddRange(dsList);
                    await _db.SaveChangesAsync();
                }

                await tx.CommitAsync();

                dto.Deal_Id = d.Deal_Id;
                dto.Image = d.Image; // return saved path
                dto.ImageBase64 = null; // clear base64 from response
                return CreatedAtAction(nameof(GetDeals), new { id = d.Deal_Id }, dto);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("deals/{id}")]
        public async Task<IActionResult> UpdateDeal(int id, DealDto dto)
        {
            var d = await _db.Deals.FindAsync(id);
            if (d == null) return NotFound();

            // If new base64 provided, save new image and update path
            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                try
                {
                    var saved = await SaveBase64ImageAsync(dto.ImageBase64, "uploads/deals");
                    d.Image = saved;
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid base64 image data.");
                }
            }

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

        [HttpGet("servicestatus")]
        public async Task<IActionResult> GetServiceStatus()
        {
            var list = await _db.BookingServiceStatuses.ToListAsync();
            return Ok(list);
        }

       
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

        // Helper: saves a base64 image to wwwroot/{folder} and returns the physical file path
        private async Task<string> SaveBase64ImageAsync(string base64, string folderRelative)
        {
            // Support data URI: data:image/png;base64,....
            var data = base64;
            var ext = "png";
            var match = Regex.Match(base64, @"data:image/(?<ext>[^;]+);base64,");
            if (match.Success)
            {
                ext = match.Groups["ext"].Value;
                data = base64.Substring(match.Value.Length);
            }
            // normalize common extensions
            if (ext.Equals("jpeg", StringComparison.OrdinalIgnoreCase)) ext = "jpg";

            byte[] bytes = Convert.FromBase64String(data); // may throw FormatException

            // Determine webroot path (wwwroot). Fallback to ContentRoot + "wwwroot".
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
            }

            var targetFolder = Path.Combine(webRoot, folderRelative.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid():N}.{ext}";
             var filePath = Path.Combine(targetFolder, fileName);
           
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);
            var shortpath = folderRelative + "/" + fileName;
            var fullpathurl = Path.GetFullPath(filePath);
            return shortpath;
        }
    }
}