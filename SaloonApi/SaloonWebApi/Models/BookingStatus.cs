using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class BookingStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}