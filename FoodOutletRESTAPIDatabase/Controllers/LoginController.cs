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
            byte[] salt = createSalt(256);
            string saltBase64tring = Convert.ToBase64String(salt);

            var user = new User
            {
                Username = userRegister.Username,
                Password = userRegister.Password,
                Role = "User",
                Salt = saltBase64tring
            };
            user.Password = HashPassword(userRegister.Password, salt); // Hash the password before saving
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

        private byte[] getuserSaltDB(User user) 
        {
            return Convert.FromBase64String(user.Salt);
        }

        private byte[] createSalt(int bits) 
        {
            return RandomNumberGenerator.GetBytes(bits / 8);
        }

        private string HashPassword(string password, byte[] salt)
        {
            string hashedpassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashedpassword;
        }

        private bool compareHashPassword(string enteredPassword, string userPassword, byte[] salt) 
        {
            if (string.Equals(userPassword, HashPassword(enteredPassword, salt)))
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
            // Retrieve saved Salt for comparing hashes
            byte[] salt = getuserSaltDB(user);
            if (!compareHashPassword(userLogin.Password, user.Password, salt)) return Unauthorized();

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

        [HttpGet("CurrentUser")]
        [Authorize]
        public IActionResult getCurrentUser()
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
