namespace FoodOutletRESTAPIDatabase.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int FoodOutletId { get; set; }
        public FoodOutlet FoodOutlet { get; set; }
        public string Comment { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
