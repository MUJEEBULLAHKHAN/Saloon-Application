using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaloonWebApi.Models
{
    public class Customer
    {
        [Key]
        public int Customer_Id { get; set; }
        public string Customer_Code { get; set; }
        public string Customer_Name { get; set; }
        [Column("S/O_W/O")]
        public string? SO_WO { get; set; }
        public string Mobile_No { get; set; }
        public string? Customer_CNIC { get; set; }
        public DateTime Registration_Date { get; set; }
        public int Branch_Id { get; set; }
        public string? Customer_Password { get; set; }
        public int? IsApp { get; set; }
        public string? Customer_Image { get; set; }
        public int? Allow_Notification { get; set; }
        public string? Customer_QR { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
        public string? Email { get; set; }
    }
}