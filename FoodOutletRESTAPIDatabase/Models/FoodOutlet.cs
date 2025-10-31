namespace FoodOutletRESTAPIDatabase.Models
{
    public class FoodOutlet
    {
        public int Id { get; init; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public List<Review> Reviews { get; init; } = [];
    }
}
