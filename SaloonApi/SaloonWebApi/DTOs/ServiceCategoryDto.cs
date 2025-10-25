
namespace SaloonWebApi.DTOs
{
    public class ServiceCategoryDto
    {
        public int Category_Id { get; set; }
        public string Category_Name { get; set; } = null!;
        public string Category_Description { get; set; } = null!;
        public string? Category_Image { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
    }
}