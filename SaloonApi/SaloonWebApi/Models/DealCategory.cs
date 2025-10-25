
using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class DealCategory
    {
        [Key]
        public int Id { get; set; }
        public string Deal_Category_Name { get; set; } = null!;
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int Modify { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}