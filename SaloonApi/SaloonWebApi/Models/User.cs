using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string Password { get; set; }
        public string Phone_No { get; set; }
        public string Email { get; set; }
        public int Branch_Id { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}