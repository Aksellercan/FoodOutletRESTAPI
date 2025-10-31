using FoodOutletRESTAPIDatabase.DTOs;
using FoodOutletRESTAPIDatabase.Models;
using FoodOutletRESTAPIDatabase.Services.Security;
using Microsoft.AspNetCore.Authorization;
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
    public class LoginController(FoodOutletDb db, IConfiguration config) : ControllerBase
    {
        readonly Password _passwordService = new();

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegister)
        {
            byte[] salt = _passwordService.CreateSalt(256);
            string saltBase64String = Convert.ToBase64String(salt);

            var user = new User
            {
                //Mail = userRegister.Mail,
                Username = userRegister.Username,
                Password = userRegister.Password,
                Role = "User",
                Salt = saltBase64String
            };
            user.Password = _passwordService.HashPassword(userRegister.Password, salt); // Hash the password before saving
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        string GenerateJwtToken(User user, IConfiguration config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    //new Claim(ClaimTypes.Email, user.Mail),
                    new(ClaimTypes.Role, user.Role),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        byte[] GetUserSaltDb(User user)
        {
            return Convert.FromBase64String(user.Salt);
        }

        private bool compareHashPassword(string enteredPassword, string userPassword, byte[] salt)
        {
            if (string.Equals(userPassword, _passwordService.HashPassword(enteredPassword, salt)))
            {
                return true;
            }
            return false;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin, IConfiguration config)
        {
            // Get first matching user
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
            if (user == null) return NotFound("login error: User not found");
            // Retrieve saved Salt for comparing hashes
            byte[] salt = GetUserSaltDb(user);
            if (!compareHashPassword(userLogin.Password, user.Password, salt)) return Unauthorized();

            var token = GenerateJwtToken(user, config);
            Response.Cookies.Append("Identity", token, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Domain = "localhost",
                Expires = DateTime.UtcNow.AddDays(1)
            });
            return Ok("Logged in Successfully");
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var claimCurrentUsername = User.FindFirst(ClaimTypes.Name);
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var claimCurrentUserRole = User.FindFirst(ClaimTypes.Role);

            var currentUsername = claimCurrentUsername?.Value;
            var currentUserIdString = claimCurrentUserId?.Value;
            var currentUserRole = claimCurrentUserRole?.Value;

            if (currentUserIdString == null || currentUsername == null || currentUserRole == null)
            {
                return BadRequest("Unauthenticated or user not found");
            }

            int currentUserId = int.Parse(claimCurrentUserId?.Value);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null)
            {
                return BadRequest("Unauthenticated or user not found");
            }

            return Ok(new UserDTO
            {
                Id = currentUserId,
                Username = currentUsername,
                Role = currentUserRole
            });
        }

        [Authorize]
        [HttpPost ("refreshToken")]
        public async Task<IActionResult> RefreshAccessToken(IConfiguration config)
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimCurrentUserId == null) { return Unauthorized("No user ID claim found."); }
            int parsedClaimId = int.Parse(claimCurrentUserId?.Value);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == parsedClaimId);
            if (user == null) return Unauthorized("User not found.");
            var token = GenerateJwtToken(user, config);
            Response.Cookies.Append("Identity", token, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Domain = "localhost",
                Expires = DateTime.UtcNow.AddDays(1)
            });
            return Ok("Token Refreshed");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult ClearCookiesLogOut()
        {
            Response.Cookies.Delete("Identity", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            return Ok("Logged out and cleared Cookies");
        }
    }
}
