namespace SaloonWebApi.DTOs
{
    public class DealServiceDto
    {
        public int Service_Id { get; set; }
        public int No_Of_Times { get; set; }
        public bool Active { get; set; } = true;
        public int? Insert_User { get; set; }

    }
}