using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class ProductCat
    {
        [Key]
        public int Category_Id { get; set; }
        public string Category_Code { get; set; } = string.Empty;
        public string Category_Name { get; set; } = string.Empty;
        public string Category_Description { get; set; } = string.Empty;
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}