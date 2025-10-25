namespace SaloonWebApi.DTOs
{
    public class ServiceDto
    {
        public int Service_Id { get; set; }
        public string Service_Code { get; set; }
        public string Service_Name { get; set; }
        public decimal Service_Price { get; set; }
        public int Service_Category { get; set; }
    }
}