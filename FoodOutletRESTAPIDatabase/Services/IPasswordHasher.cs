namespace FoodOutletRESTAPIDatabase.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, byte[] salt);
        byte[] createSalt(int bits);
    }
}
