namespace SaloonWebApi.DTOs
{
    public class CreateProductDto
    {
        public string Product_Code { get; set; } = string.Empty;
        public string Product_Name { get; set; } = string.Empty;
        public string? Prod_Name { get; set; }
        public string Product_Description { get; set; } = string.Empty;
        public int Brand { get; set; }
        public string? Weight { get; set; }
        public int? Unit { get; set; }
        public int Category { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Company_Price { get; set; }
        public decimal Min_Price { get; set; }
        public decimal Max_Price { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Sale_Price { get; set; }
        public string? Product_Image { get; set; }
        // base64-encoded image payload (optional). Accepts plain base64 or data URI (data:image/png;base64,...)
        public string? Product_ImageBase64 { get; set; }
        public string Account_No { get; set; } = string.Empty;
        public int? No_of_Times { get; set; }
        public string? Barcode { get; set; }
    }
}