namespace FoodOutletRESTAPIDatabase.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public required string Comment { get; set; }
        public int Score { get; set; }
        public int FoodOutletId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
