using FoodOutletRESTAPIDatabase.DTOs;
using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly FoodOutletDb _db;
        private readonly IConfiguration _config;

        public LoginController(FoodOutletDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegister)
        {
            var user = new User
            {
                Username = userRegister.Username,
                Password = userRegister.Password,
                Role = "User"
            };
            Console.WriteLine($"Before hashing: {user.Password}");
            user.Password = HashPassword(userRegister.Password); // Hash the password before saving
            Console.WriteLine($"Hashed password: {user.Password}");

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        private string GenerateJwtToken(User user, IConfiguration config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password) { 
            byte[] salt = RandomNumberGenerator.GetBytes(128/8);
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
            string hashedpassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8));
            Console.WriteLine($"Hashed: {hashedpassword}");
            return hashedpassword;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin, IConfiguration config)
        {   
            
            //get first matching user and assign it to var user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);

            // Plain password check (no hash to be added later)
            if (user == null || user.Password != userLogin.Password)
            {
                return Unauthorized();
            }

            // Generate JWT Token using config from Program.cs
            var token = GenerateJwtToken(user, config);
            return Ok(new { Token = token });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyAdmin() {
            Console.WriteLine("Admin Role Verified!");
            return Ok();
        }
    }
}
