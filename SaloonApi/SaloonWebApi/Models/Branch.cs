using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public string Branch_Name { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}