namespace FoodOutletRESTAPIDatabase.Models
{
    public class Review
    {
        public int Id { get; init; }
        public int FoodOutletId { get; init; }
        public FoodOutlet? FoodOutlet { get; set; }
        public required string Comment { get; init; }
        public int Score { get; init; }
        public int UserId { get; set; }
        public User? User { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
    }
}
