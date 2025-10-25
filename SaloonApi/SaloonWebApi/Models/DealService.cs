using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class DealService
    {
        [Key]
        public int Deal_Service_Id { get; set; }
        public int Deal_Id { get; set; }
        public int Service_Id { get; set; }
        public int No_Of_Times { get; set; }
        public bool Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Update_User { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}