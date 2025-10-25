namespace SaloonWebApi.DTOs
{
    public class BookingServiceDto
    {
        public int Service_Id { get; set; }
        public decimal ServicePrice { get; set; }
        public int? ServiceProvider_Id { get; set; }
        public int? Deal_Id { get; set; }
        public decimal? DealPrice { get; set; }
        public int Status_Id { get; set; }
    }
}