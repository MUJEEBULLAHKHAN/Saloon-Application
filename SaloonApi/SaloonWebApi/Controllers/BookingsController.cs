using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.Models;
using SaloonWebApi.DTOs;

namespace SaloonWebApi.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        public BookingsController(SaloonDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _db.BookingMasters.ToListAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _db.BookingMasters.FindAsync(id);
            if (booking == null) return NotFound();
            var details = await _db.BookingDetails.Where(d => d.BookingMaster_Id == id).ToListAsync();
            return Ok(new { booking, details });
        }

        // Persist booking + details and allow optional serviceman assignment per service
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            if (dto.Services == null || !dto.Services.Any())
                return BadRequest("At least one service must be provided.");

            // Use transaction to ensure master + details saved atomically
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var booking = new BookingMaster
                {
                    BookingNo = dto.BookingNo,
                    CustomerPhone_No = dto.CustomerPhone_No,
                    Customer_Id = dto.Customer_Id,
                    BookingDate = dto.BookingDate,
                    BookingTime = dto.BookingTime,
                    Branch_Id = dto.Branch_Id,
                    Remarks = dto.Remarks,
                    Booking_Status_Id = dto.Booking_Status_Id,
                    Insert_Date = DateTime.UtcNow,
                    Active = 1
                };

                // Add master to get identity id
                _db.BookingMasters.Add(booking);
                await _db.SaveChangesAsync(); // booking.Id populated

                var details = new List<BookingDetail>();
                foreach (var s in dto.Services)
                {
                    var detail = new BookingDetail
                    {
                        BookingMaster_Id = booking.Id,
                        Service_Id = s.Service_Id,
                        ServicePrice = s.ServicePrice,
                        ServiceProvider_Id = s.ServiceProvider_Id ?? 0,
                        ProviderAssignDate = (s.ServiceProvider_Id.HasValue && s.ServiceProvider_Id.Value > 0) ? DateTime.UtcNow : null,
                        Deal_Id = s.Deal_Id,
                        DealPrice = s.DealPrice,
                        Status_Id = s.Status_Id,
                        InsertUser_Id = 0,
                        Insert_Date = DateTime.UtcNow,
                        Active = 1
                    };
                    details.Add(detail);
                }

                _db.BookingDetails.AddRange(details);
                await _db.SaveChangesAsync();

                // compute total amount (allow client-provided override)
                var computedTotal = details.Sum(d => d.ServicePrice) + details.Where(d => d.DealPrice.HasValue).Sum(d => d.DealPrice ?? 0m);
                booking.Total_Amount = dto.Total_Amount ?? computedTotal;

                // persist booking total update
                _db.BookingMasters.Update(booking);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, new { booking, details });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        // Add one or more services to an existing booking
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddServices(int id, [FromBody] List<BookingServiceDto> services)
        {
            if (services == null || !services.Any())
                return BadRequest("At least one service must be provided.");

            var booking = await _db.BookingMasters.FindAsync(id);
            if (booking == null) return NotFound();

            // Optional: validate that provided Service_Id values exist in Services table.
            var serviceIds = services.Select(s => s.Service_Id).Distinct().ToList();
            var existingServiceIds = await _db.Services.Where(s => serviceIds.Contains(s.Service_Id)).Select(s => s.Service_Id).ToListAsync();
            if (existingServiceIds.Count != serviceIds.Count)
                return BadRequest("One or more Service_Id values are invalid.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = new List<BookingDetail>();
                foreach (var s in services)
                {
                    var detail = new BookingDetail
                    {
                        BookingMaster_Id = id,
                        Service_Id = s.Service_Id,
                        ServicePrice = s.ServicePrice,
                        ServiceProvider_Id = s.ServiceProvider_Id ?? 0,
                        ProviderAssignDate = (s.ServiceProvider_Id.HasValue && s.ServiceProvider_Id.Value > 0) ? DateTime.UtcNow : null,
                        Deal_Id = s.Deal_Id,
                        DealPrice = s.DealPrice,
                        Status_Id = s.Status_Id,
                        InsertUser_Id = 0,
                        Insert_Date = DateTime.UtcNow,
                        Active = 1
                    };
                    details.Add(detail);
                }

                _db.BookingDetails.AddRange(details);
                await _db.SaveChangesAsync();

                // Update booking total
                var addAmount = details.Sum(d => d.ServicePrice) + details.Where(d => d.DealPrice.HasValue).Sum(d => d.DealPrice ?? 0m);
                booking.Total_Amount += addAmount;
                _db.BookingMasters.Update(booking);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                // return created details along with updated booking
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, new { booking, added = details });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // Remove a service (booking detail) from a booking
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{id}/services/{detailId}")]
        public async Task<IActionResult> RemoveService(int id, int detailId)
        {
            var detail = await _db.BookingDetails.FindAsync(detailId);
            if (detail == null) return NotFound();
            if (detail.BookingMaster_Id != id) return BadRequest("Detail does not belong to the specified booking.");

            var booking = await _db.BookingMasters.FindAsync(id);
            if (booking == null) return NotFound();

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // compute amount to subtract
                var removeAmount = detail.ServicePrice + (detail.DealPrice ?? 0m);

                _db.BookingDetails.Remove(detail);
                await _db.SaveChangesAsync();

                booking.Total_Amount -= removeAmount;
                if (booking.Total_Amount < 0) booking.Total_Amount = 0;
                _db.BookingMasters.Update(booking);
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingMaster model)
        {
            var existing = await _db.BookingMasters.FindAsync(id);
            if (existing == null) return NotFound();
            existing.BookingDate = model.BookingDate;
            existing.BookingTime = model.BookingTime;
            existing.Total_Amount = model.Total_Amount;
            existing.Remarks = model.Remarks;
            existing.ModifyDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusId)
        {
            var existing = await _db.BookingMasters.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Booking_Status_Id = statusId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignServicePerson(int id, [FromBody] BookingsAssignDto dto)
        {
            var detail = await _db.BookingDetails.FindAsync(dto.BookingDetailId);
            if (detail == null || detail.BookingMaster_Id != id) return NotFound();
            detail.ServiceProvider_Id = dto.ServiceProviderId;
            detail.ProviderAssignDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("by-role")]
        public async Task<IActionResult> GetByRole([FromQuery] int roleId)
        {
            // simple filter by status or branch could be implemented
            var list = await _db.BookingMasters.Where(b => b.Booking_Status_Id == roleId).ToListAsync();
            return Ok(list);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseBooking(int id)
        {
            var booking = await _db.BookingMasters.FindAsync(id);
            if (booking == null) return NotFound();
            booking.Booking_Status_Id = 3; // assume 3=Closed/Paid
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public class BookingsAssignDto
    {
        public int BookingDetailId { get; set; }
        public int ServiceProviderId { get; set; }
    }
}