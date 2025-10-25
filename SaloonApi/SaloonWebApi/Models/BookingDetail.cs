using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class BookingDetail
    {
        [Key]
        public int Id { get; set; }
        public int BookingMaster_Id { get; set; }
        public int Service_Id { get; set; }
        public decimal ServicePrice { get; set; }
        public int ServiceProvider_Id { get; set; }
        public int Active { get; set; }
        public int InsertUser_Id { get; set; }
        public DateTime Insert_Date { get; set; }
        public int Modified { get; set; }
        public int? ModifyUser_Id { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? Deal_Id { get; set; }
        public decimal? DealPrice { get; set; }
        public int Status_Id { get; set; }
        public DateTime? Service_Start_Time { get; set; }
        public DateTime? Service_End_Time { get; set; }
        public DateTime? ProviderAssignDate { get; set; }
    }
}