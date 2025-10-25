using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class UserRole
    {
        [Key]
        public int UserRole_Id { get; set; }
        public int? User_Id { get; set; }
        public int? Role_Id { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}