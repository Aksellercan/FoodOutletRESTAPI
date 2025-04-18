using FoodOutletRESTAPIDatabase.DTOs;
using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        private string HashPassword(string password)
        {
            //byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            byte[] salt = new byte[16];
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
            string hashedpassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed: {hashedpassword}");
            return hashedpassword;
        }

        private bool DeHashPassword(string enteredPassword, string userPassword) 
        {
            string hashedpassword = HashPassword(enteredPassword);
            Console.WriteLine($"Hashed: {hashedpassword}");
            if (string.Equals(userPassword,hashedpassword))
            {
                return true;
            }
            return false;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin, IConfiguration config)
        {
            // Get first matching user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);

            if (user == null) return NotFound("login error: User not found");
            // Check by comparing hashes (Not secure at the moment)
            if (!DeHashPassword(userLogin.Password, user.Password)) return Unauthorized();

            var token = GenerateJwtToken(user, config);
            return Ok(new { Token = token });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> getUser([FromRoute] int userId) 
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("User not found");
            return Ok(user.Username);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult VerifyAdmin()
        {
            Console.WriteLine("Admin Role Verified!");
            return Ok();
        }

        // This endpoint is for testing purposes only. It returns the current user's name to display on homepage.
        [HttpGet("testPoint")]
        [Authorize]
        public IActionResult getUserCurrently()
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.Name);
            var currentUserId = claimCurrentUserId?.Value;
            if (currentUserId == null)
            {
                return BadRequest("Unauthenticated or user not found");
            }
            return Ok(currentUserId);
        }
    }
}
