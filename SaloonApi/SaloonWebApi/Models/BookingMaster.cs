using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class BookingMaster
    {
        [Key]
        public int Id { get; set; }
        public string? BookingNo { get; set; }
        public string? CustomerPhone_No { get; set; }
        public int? Customer_Id { get; set; }
        public DateTime? BookingDate { get; set; }
        public TimeSpan? BookingTime { get; set; }
        public int? Branch_Id { get; set; }
        public int? Active { get; set; }
        public int? InsertUser_Id { get; set; }
        public DateTime? Insert_Date { get; set; }
        public int? Modified { get; set; }
        public int? ModifyUser_Id { get; set; }
        public DateTime? ModifyDate { get; set; }
        public decimal Total_Amount { get; set; }
        public string? Remarks { get; set; }
        public int? Booking_Status_Id { get; set; }
    }
}