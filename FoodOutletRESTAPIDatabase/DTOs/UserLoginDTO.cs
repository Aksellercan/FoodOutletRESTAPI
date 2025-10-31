namespace FoodOutletRESTAPIDatabase.DTOs
{
    public class UserLoginDTO
    {
        //public string Mail { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}