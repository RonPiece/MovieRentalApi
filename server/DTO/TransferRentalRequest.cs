namespace hw4.DTO
{
    public class TransferRentalRequest
    {
        public int RentalId { get; set; }
        public string ToUserEmail { get; set; } = string.Empty;
    }
}