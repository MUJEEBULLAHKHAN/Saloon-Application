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
    public class ServiceSaleController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public ServiceSaleController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await (from m in _db.ServiceSaleMasters
                                  join c in _db.Customers on m.Customer_Id equals c.Customer_Id into cj
                                  from c in cj.DefaultIfEmpty()
                                  join e in _db.Employees on m.Service_Person_Id equals e.Employee_Id into ej
                                  from e in ej.DefaultIfEmpty()
                                  select new
                                  {
                                      Master = m,
                                      CustomerName = c != null ? c.Customer_Name : null,
                                      ServicePersonName = e != null
                                          ? (string.IsNullOrWhiteSpace(e.Employee_LastName) ? e.Employee_Name : e.Employee_Name + " " + e.Employee_LastName)
                                          : null
                                  }).ToListAsync();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var master = await _db.ServiceSaleMasters.FindAsync(id);
                if (master == null) return NotFound();

                Customer? customer = null;
                if (master.Customer_Id.HasValue)
                {
                    customer = await _db.Customers
                        .Where(c => c.Customer_Id == master.Customer_Id.Value)
                        .Select(c => new Customer
                        {
                            Customer_Id = c.Customer_Id,
                            Customer_Name = c.Customer_Name,
                            Registration_Date = c.Registration_Date
                        })
                        .FirstOrDefaultAsync();
                }

                var details = await (from d in _db.ServiceSaleDetails
                                     where d.Service_Master_Id == id
                                     join s in _db.Services on d.Service_Id equals s.Service_Id into sj
                                     from s in sj.DefaultIfEmpty()
                                     join e in _db.Employees on d.Service_Person_Id equals e.Employee_Id into ej
                                     from e in ej.DefaultIfEmpty()
                                     select new
                                     {
                                         Detail = d,
                                         ServiceName = s != null ? s.Service_Name : null,
                                         ServicePersonName = e != null
                                             ? (string.IsNullOrWhiteSpace(e.Employee_LastName)
                                                 ? e.Employee_Name
                                                 : (e.Employee_Name + " " + e.Employee_LastName))
                                             : null
                                     }).ToListAsync();

                return Ok(new
                {
                    Master = master,
                    CustomerName = customer?.Customer_Name,
                    CustomerRegistrationDate = customer?.Registration_Date,
                    Details = details
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceSaleDto dto)
        {
            if (dto.Details == null || !dto.Details.Any())
                return BadRequest("At least one detail must be provided.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var master = new ServiceSaleMaster
                {
                    Service_No = dto.Service_No ?? string.Empty,
                    Customer_Type = dto.Customer_Type,
                    Customer_Id = dto.Customer_Id,
                    Customer_Name = dto.Customer_Name ?? string.Empty,
                    Account_No = dto.Account_No ?? string.Empty,
                    Branch = dto.Branch,
                    Total_Amount = dto.Total_Amount ?? 0m,
                    Receive_Amount = dto.Receive_Amount ?? 0m,
                    Service_Person_Id = dto.Service_Person_Id,
                    Sale_Date = dto.Sale_Date,
                    PaymentMethod_Id = dto.PaymentMethod_Id,
                    Reference_No = dto.Reference_No,
                    Cash_Amount = dto.Cash_Amount,
                    Bank_Amount = dto.Bank_Amount,
                    Start_Time = dto.Start_Time,
                    End_Time = dto.End_Time,
                    AppointmentNo = dto.AppointmentNo,
                    Advance_Amount = dto.Advance_Amount,
                    Active = dto.Active,
                    Insert_User = dto.Insert_User,
                    Insert_Date = DateTime.UtcNow,
                    IsEmployee = dto.IsEmployee,
                    Credit_Amount = dto.Credit_Amount,
                    Redeem_Points = dto.Redeem_Points,
                    TipInBank = dto.TipInBank,
                    Auto_Manual = dto.Auto_Manual
                };

                _db.ServiceSaleMasters.Add(master);
                await _db.SaveChangesAsync();

                var details = dto.Details.Select(d => new ServiceSaleDetail
                {
                    Service_Master_Id = master.Service_Id,
                    Service_Id = d.Service_Id,
                    Price = d.Price,
                    Service_Person_Id = d.Service_Person_Id,
                    Active = d.Active,
                    Insert_User = d.Insert_User,
                    Insert_Date = DateTime.UtcNow,
                    Cash = d.Cash,
                    Bank = d.Bank
                }).ToList();

                _db.ServiceSaleDetails.AddRange(details);
                await _db.SaveChangesAsync();

                var computedTotal = details.Sum(x => x.Price);
                master.Total_Amount = dto.Total_Amount ?? computedTotal;
                _db.ServiceSaleMasters.Update(master);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = master.Service_Id }, new { master, details });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddServices(int id, [FromBody] List<ServiceSaleDetailCreateDto> services)
        {
            if (services == null || !services.Any())
                return BadRequest("At least one service must be provided.");

            var master = await _db.ServiceSaleMasters.FindAsync(id);
            if (master == null) return NotFound();

            var serviceIds = services.Select(s => s.Service_Id).Distinct().ToList();
            var existingServiceIds = await _db.Services.Where(s => serviceIds.Contains(s.Service_Id)).Select(s => s.Service_Id).ToListAsync();
            if (existingServiceIds.Count != serviceIds.Count)
                return BadRequest("One or more Service_Id values are invalid.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = services.Select(s => new ServiceSaleDetail
                {
                    Service_Master_Id = id,
                    Service_Id = s.Service_Id,
                    Price = s.Price,
                    Service_Person_Id = s.Service_Person_Id,
                    Active = s.Active,
                    Insert_User = s.Insert_User,
                    Insert_Date = DateTime.UtcNow,
                    Cash = s.Cash,
                    Bank = s.Bank
                }).ToList();

                _db.ServiceSaleDetails.AddRange(details);
                await _db.SaveChangesAsync();

                master.Total_Amount += details.Sum(x => x.Price);
                _db.ServiceSaleMasters.Update(master);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = master.Service_Id }, new { master, added = details });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [HttpDelete("{id}/services/{detailId}")]
        public async Task<IActionResult> RemoveService(int id, int detailId)
        {
            // this method already uses try/catch transaction in original implementation
            var detail = await _db.ServiceSaleDetails.FindAsync(detailId);
            if (detail == null) return NotFound();
            if (detail.Service_Master_Id != id) return BadRequest("Detail does not belong to the specified sale.");

            var master = await _db.ServiceSaleMasters.FindAsync(id);
            if (master == null) return NotFound();

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var removeAmount = detail.Price;

                _db.ServiceSaleDetails.Remove(detail);
                await _db.SaveChangesAsync();

                master.Total_Amount -= removeAmount;
                if (master.Total_Amount < 0) master.Total_Amount = 0;
                _db.ServiceSaleMasters.Update(master);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceSaleDto dto)
        {
            try
            {
                var existing = await _db.ServiceSaleMasters.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Sale_Date = dto.Sale_Date ?? existing.Sale_Date;
                existing.Start_Time = dto.Start_Time ?? existing.Start_Time;
                existing.End_Time = dto.End_Time ?? existing.End_Time;
                existing.Total_Amount = dto.Total_Amount ?? existing.Total_Amount;
                existing.Receive_Amount = dto.Receive_Amount ?? existing.Receive_Amount;
                existing.Reference_No = dto.Reference_No ?? existing.Reference_No;
                if (dto.Active.HasValue) existing.Active = dto.Active.Value;
                existing.Update_Date = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int active)
        {
            try
            {
                var existing = await _db.ServiceSaleMasters.FindAsync(id);
                if (existing == null) return NotFound();
                existing.Active = active == 1;
                existing.Update_Date = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignServicePerson(int id, [FromBody] ServiceSaleAssignDto dto)
        {
            try
            {
                var detail = await _db.ServiceSaleDetails.FindAsync(dto.ServiceDetailId);
                if (detail == null || detail.Service_Master_Id != id) return NotFound();
                detail.Service_Person_Id = dto.ServicePersonId;
                detail.Update_Date = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("by-role")]
        public async Task<IActionResult> GetByRole([FromQuery] int roleId)
        {
            try
            {
                var list = await _db.ServiceSaleMasters.Where(m => m.Service_Person_Id == roleId).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseSale(int id)
        {
            try
            {
                var existing = await _db.ServiceSaleMasters.FindAsync(id);
                if (existing == null) return NotFound();
                existing.Active = false;
                existing.Update_Date = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
