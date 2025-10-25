using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class Service
    {
        [Key]
        public int Service_Id { get; set; }
        public string Service_Code { get; set; }
        public string Service_Name { get; set; }
        public string Service_Description { get; set; }
        public decimal Service_Price { get; set; }
        public int Service_Category { get; set; }
        public int Gender { get; set; }
        public string? Service_Image { get; set; }
        public int? Points { get; set; }
        public int? Total_Points { get; set; }
        public decimal? Min_Amount { get; set; }
        public decimal? Discounted_Price { get; set; }
        public decimal? App_Price { get; set; }
        public string Account_No { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}