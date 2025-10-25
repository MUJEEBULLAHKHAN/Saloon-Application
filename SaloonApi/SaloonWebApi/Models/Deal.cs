using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class Deal
    {
        [Key]
        public int Deal_Id { get; set; }
        public string Deal_Name { get; set; }
        public decimal Deal_Price { get; set; }
        public int Deal_Type { get; set; }
        public int? Deal_Cat_Id { get; set; }
        public bool Active { get; set; }
        public DateTime Insert_Date { get; set; }
        public int Insert_User { get; set; }
        public DateTime? Update_Date { get; set; }
        public int? Update_User { get; set; }
        public string? Image { get; set; }
    }
}