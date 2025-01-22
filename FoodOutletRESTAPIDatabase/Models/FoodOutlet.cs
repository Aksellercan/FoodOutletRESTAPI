namespace FoodOutletRESTAPIDatabase.Models
{
    public class FoodOutlet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        //public int Rating { get; set; }
        public List<Review> Reviews { get; set; } = new();
    }
}
