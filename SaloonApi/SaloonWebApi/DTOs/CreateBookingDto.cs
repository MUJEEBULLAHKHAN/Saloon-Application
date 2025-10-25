using System.Collections.Generic;

namespace SaloonWebApi.DTOs
{
    public class CreateBookingDto
    {
        public string? BookingNo { get; set; }
        public string? CustomerPhone_No { get; set; }
        public int? Customer_Id { get; set; }
        public DateTime? BookingDate { get; set; }
        public TimeSpan? BookingTime { get; set; }
        public int? Branch_Id { get; set; }
        public decimal? Total_Amount { get; set; }
        public string? Remarks { get; set; }
        public int? Booking_Status_Id { get; set; }

        // At least one service is expected
        public List<BookingServiceDto> Services { get; set; } = new();
    }
}