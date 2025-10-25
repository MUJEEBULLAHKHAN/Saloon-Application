namespace SaloonWebApi.DTOs
{
    public class CreateCustomerDto
    {
        public string Customer_Code { get; set; }
        public string Customer_Name { get; set; }
        public string Mobile_No { get; set; }
        public string? Email { get; set; }
    }
}