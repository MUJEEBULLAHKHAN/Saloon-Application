using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class ServiceSaleMaster
    {
        [Key]
        public int Service_Id { get; set; }
        public string Service_No { get; set; } = string.Empty;
        public int Customer_Type { get; set; }
        public int? Customer_Id { get; set; }
        public string Customer_Name { get; set; } = string.Empty;
        public string Account_No { get; set; } = string.Empty;
        public int Branch { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal Receive_Amount { get; set; }
        public int? Service_Person_Id { get; set; }
        public DateTime? Sale_Date { get; set; }
        public int? PaymentMethod_Id { get; set; }
        public string? Reference_No { get; set; }
        public decimal? Cash_Amount { get; set; }
        public decimal? Bank_Amount { get; set; }
        public DateTime? Start_Time { get; set; }
        public DateTime? End_Time { get; set; }
        public string? AppointmentNo { get; set; }
        public decimal? Advance_Amount { get; set; }
        public bool Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Update_User { get; set; }
        public DateTime? Update_Date { get; set; }
        public bool? IsEmployee { get; set; }
        public decimal? Credit_Amount { get; set; }
        public int? Redeem_Points { get; set; }
        public decimal? TipInBank { get; set; }
        public int? Auto_Manual { get; set; }
    }
}
