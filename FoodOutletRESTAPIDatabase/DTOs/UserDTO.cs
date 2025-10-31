namespace FoodOutletRESTAPIDatabase.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        //public string Mail { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}
