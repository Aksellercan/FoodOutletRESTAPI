using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FoodOutletRESTAPIDatabase.Services
{
    public class Password : IPasswordHasher
    {
        public string HashPassword(string password, byte[] salt) 
        {
            string hashedpassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            Console.WriteLine($"current hash password {hashedpassword}");
            return hashedpassword;
        }

        public byte[] createSalt(int bits) 
        {
            byte[] test = RandomNumberGenerator.GetBytes(bits / 8);
            Console.WriteLine($"current salt = {test}");
            return test;
        }
    }
}