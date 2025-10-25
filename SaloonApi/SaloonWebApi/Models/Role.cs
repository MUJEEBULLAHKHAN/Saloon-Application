using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class Role
    {
        [Key]
        public int Role_Id { get; set; }
        public string Role_Name { get; set; }
        public bool Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}