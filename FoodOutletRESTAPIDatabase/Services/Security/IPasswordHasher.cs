namespace FoodOutletRESTAPIDatabase.Services.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, byte[] salt);
        byte[] CreateSalt(int bits);
    }
}
