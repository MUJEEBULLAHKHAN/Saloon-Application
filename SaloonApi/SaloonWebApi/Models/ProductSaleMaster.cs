using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class ProductSaleMaster
    {
        [Key]
        public int Sale_Id { get; set; }
        public string Sale_No { get; set; } = string.Empty;
        public int? Customer_Id { get; set; }
        public string Customer_Name { get; set; } = string.Empty;
        public string? Customer_Mobile { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal? Receive_Amount { get; set; }
        public DateTime Sale_Date { get; set; }
        public string? Account_No { get; set; }
        public int? PaymentMethod_Id { get; set; }
        public string? Reference_No { get; set; }
        public int Branch_Id { get; set; }
        public decimal? Cash_Amount { get; set; }
        public decimal? Bank_Amount { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Update_User { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}