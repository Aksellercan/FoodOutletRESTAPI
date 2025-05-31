namespace FoodOutletRESTAPIDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        //public string Mail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; } //save salt along side with user
        public string Role { get; set; }
        public List<Review> Reviews { get; set; } = new();
    }
}
