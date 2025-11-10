using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class ProductSaleDetail
    {
        [Key]
        public int SaleDetail_Id { get; set; }
        public int SaleMaster_Id { get; set; }
        public int? Product_Id { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Amount { get; set; }
        public int? Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Update_User { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}