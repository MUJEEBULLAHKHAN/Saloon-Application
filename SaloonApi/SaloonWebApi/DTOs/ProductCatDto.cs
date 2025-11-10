namespace SaloonWebApi.DTOs
{
    public class ProductCatDto
    {
        public int Category_Id { get; set; }
        public string Category_Code { get; set; } = string.Empty;
        public string Category_Name { get; set; } = string.Empty;
        public string Category_Description { get; set; } = string.Empty;
    }
}