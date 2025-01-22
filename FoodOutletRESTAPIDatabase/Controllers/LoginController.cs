using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                Password = userRegister.Password, // Storing plain text password (no hashing)
                Role = "User"
            };

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
                },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin, IConfiguration config)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
            if (user == null || user.Password != userLogin.Password) // Plain password check
            {
                return Unauthorized();
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user, config);
            return Ok(new { Token = token });
        }


    }
}
