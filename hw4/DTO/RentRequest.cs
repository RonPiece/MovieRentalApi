namespace hw4.DTO
{
    public class RentRequest
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
    }
}
