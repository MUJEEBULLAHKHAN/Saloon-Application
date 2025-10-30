using System.Collections.Generic;

namespace SaloonWebApi.DTOs
{
    public class DealDto
    {
        public int Deal_Id { get; set; }
        public string Deal_Name { get; set; } = null!;
        public decimal Deal_Price { get; set; }
        public int Deal_Type { get; set; }
        public int? Deal_Cat_Id { get; set; }
        public bool Active { get; set; } = true;

        // Database path for the saved image (returned)
        public string? Image { get; set; }

        // Incoming base64 image string (data URI or raw base64). Not persisted.
        public string? ImageBase64 { get; set; }

        public int? Insert_User { get; set; }

        // Incoming/returned service list for the deal
        public List<DealServiceDto> Services { get; set; } = new();
    }
}