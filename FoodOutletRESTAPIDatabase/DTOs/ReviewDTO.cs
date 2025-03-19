namespace FoodOutletRESTAPIDatabase.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Score { get; set; }
        public int FoodoutletId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
