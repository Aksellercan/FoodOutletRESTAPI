using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FoodOutletRESTAPIDatabase.Services.Security
{
    public class Password : IPasswordHasher
    {
        public string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public byte[] CreateSalt(int bits)
        {
            return RandomNumberGenerator.GetBytes(bits / 8);
        }
    }
}