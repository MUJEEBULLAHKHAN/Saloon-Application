using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductSaleController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public ProductSaleController(SaloonDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.ProductSaleMasters.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var master = await _db.ProductSaleMasters.FindAsync(id);
            if (master == null) return NotFound();

            var details = await _db.ProductSaleDetails.Where(d => d.SaleMaster_Id == id)
                .Join(_db.Products, d => d.Product_Id, p => p.Product_Id, (d, p) => new { Detail = d, ProductName = p.Product_Name })
                .Select(x => new { x.Detail, x.ProductName })
                .ToListAsync();

            return Ok(new { master, details });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductSaleDto dto)
        {
            if (dto == null || dto.Details == null || !dto.Details.Any())
                return BadRequest("Invalid sale data.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var master = new ProductSaleMaster
                {
                    Sale_No = dto.Sale_No,
                    Customer_Id = dto.Customer_Id,
                    Customer_Name = dto.Customer_Name,
                    Customer_Mobile = dto.Customer_Mobile,
                    Total_Amount = dto.Total_Amount,
                    Receive_Amount = dto.Receive_Amount,
                    Sale_Date = dto.Sale_Date,
                    Account_No = dto.Account_No,
                    PaymentMethod_Id = dto.PaymentMethod_Id,
                    Reference_No = dto.Reference_No,
                    Branch_Id = dto.Branch_Id,
                    Cash_Amount = dto.Cash_Amount,
                    Bank_Amount = dto.Bank_Amount,
                    Active = 1,
                    Insert_User = 0,
                    Insert_Date = DateTime.UtcNow
                };

                _db.ProductSaleMasters.Add(master);
                await _db.SaveChangesAsync();

                var details = new List<ProductSaleDetail>();
                foreach (var d in dto.Details)
                {
                    var det = new ProductSaleDetail
                    {
                        SaleMaster_Id = master.Sale_Id,
                        Product_Id = d.Product_Id,
                        Qty = d.Qty,
                        Price = d.Price,
                        Total_Amount = d.Price * d.Qty,
                        Active = 1,
                        Insert_User = 0,
                        Insert_Date = DateTime.UtcNow
                    };
                    details.Add(det);
                }

                _db.ProductSaleDetails.AddRange(details);
                await _db.SaveChangesAsync();

                // compute total and update master if mismatch
                var computedTotal = details.Sum(x => x.Total_Amount);
                master.Total_Amount = dto.Total_Amount == 0 ? computedTotal : dto.Total_Amount;
                _db.ProductSaleMasters.Update(master);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return CreatedAtAction(nameof(GetById), new { id = master.Sale_Id }, new { master, details });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductSaleDto dto)
        {
            var existing = await _db.ProductSaleMasters.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Sale_No = dto.Sale_No;
            existing.Customer_Id = dto.Customer_Id;
            existing.Customer_Name = dto.Customer_Name;
            existing.Customer_Mobile = dto.Customer_Mobile;
            existing.Total_Amount = dto.Total_Amount;
            existing.Receive_Amount = dto.Receive_Amount;
            existing.Sale_Date = dto.Sale_Date;
            existing.Account_No = dto.Account_No;
            existing.PaymentMethod_Id = dto.PaymentMethod_Id;
            existing.Reference_No = dto.Reference_No;
            existing.Branch_Id = dto.Branch_Id;
            existing.Cash_Amount = dto.Cash_Amount;
            existing.Bank_Amount = dto.Bank_Amount;
            existing.Update_User = dto.Details.Any() ? 0 : existing.Update_User;
            existing.Update_Date = DateTime.UtcNow;

            _db.ProductSaleMasters.Update(existing);

            // replace details: simple approach - delete existing and insert provided
            var oldDetails = await _db.ProductSaleDetails.Where(d => d.SaleMaster_Id == id).ToListAsync();
            _db.ProductSaleDetails.RemoveRange(oldDetails);

            var newDetails = dto.Details.Select(d => new ProductSaleDetail
            {
                SaleMaster_Id = id,
                Product_Id = d.Product_Id,
                Qty = d.Qty,
                Price = d.Price,
                Total_Amount = d.Total_Amount,
                Insert_User = 0,
                Insert_Date = DateTime.UtcNow
            }).ToList();

            _db.ProductSaleDetails.AddRange(newDetails);

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var master = await _db.ProductSaleMasters.FindAsync(id);
            if (master == null) return NotFound();

            var details = await _db.ProductSaleDetails.Where(d => d.SaleMaster_Id == id).ToListAsync();

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.ProductSaleDetails.RemoveRange(details);
                _db.ProductSaleMasters.Remove(master);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var master = await _db.ProductSaleMasters.FindAsync(id);
            if (master == null) return NotFound();

            var details = await _db.ProductSaleDetails.Where(d => d.SaleMaster_Id == id)
                .Join(_db.Products, d => d.Product_Id, p => p.Product_Id, (d, p) => new ProductSaleDetail()
                {
                    SaleDetail_Id = d.SaleDetail_Id,
                    SaleMaster_Id = d.SaleMaster_Id,
                    Product_Id = d.Product_Id,
                    Qty = d.Qty,
                    Price = d.Price,
                    Total_Amount = d.Total_Amount,
                    Insert_Date = d.Insert_Date
                })
                .ToListAsync();

            return Ok(details);
        }
    }
}
