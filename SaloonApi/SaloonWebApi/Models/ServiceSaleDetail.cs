using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class ServiceSaleDetail
    {
        [Key]
        public int Service_Detail_Id { get; set; }
        public int Service_Master_Id { get; set; }
        public int Service_Id { get; set; }
        public decimal Price { get; set; }
        public int? Service_Person_Id { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Update_User { get; set; }
        public DateTime? Update_Date { get; set; }
        public decimal? Cash { get; set; }
        public decimal? Bank { get; set; }
    }
}
