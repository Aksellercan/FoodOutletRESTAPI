namespace FoodOutletRESTAPIDatabase.Models
{
    public class User
    {
        public int Id { get; init; }
        //public string Mail { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Salt { get; set; } //save salt along side with user
        public required string Role { get; init; }
        public List<Review> Reviews { get; init; } = [];
    }
}
