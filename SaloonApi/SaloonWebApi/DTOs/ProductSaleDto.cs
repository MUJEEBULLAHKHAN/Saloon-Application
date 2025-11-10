namespace SaloonWebApi.DTOs
{
    public class ProductSaleDto
    {
        public int Sale_Id { get; set; }
        public string Sale_No { get; set; } = string.Empty;
        public int? Customer_Id { get; set; }
        public string Customer_Name { get; set; } = string.Empty;
        public string? Customer_Mobile { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal? Receive_Amount { get; set; }
        public DateTime Sale_Date { get; set; }
        public string? Account_No { get; set; }
        public int? PaymentMethod_Id { get; set; }
        public string? Reference_No { get; set; }
        public int Branch_Id { get; set; }
        public decimal? Cash_Amount { get; set; }
        public decimal? Bank_Amount { get; set; }
        public List<ProductSaleDetailDto> Details { get; set; } = new();
    }

    public class ProductSaleDetailDto
    {
        public int SaleDetail_Id { get; set; }
        public int SaleMaster_Id { get; set; }
        public int? Product_Id { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total_Amount { get; set; }
    }
}