namespace SaloonWebApi.DTOs
{
    public class DealDto
    {
        public int Deal_Id { get; set; }
        public string Deal_Name { get; set; }
        public decimal Deal_Price { get; set; }
        public bool Active { get; set; }
    }
}