﻿using FoodOutletRESTAPIDatabase.DTOs;
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
    public class LoginController : ControllerBase
    {
        private readonly FoodOutletDb _db;
        private readonly IConfiguration _config;
        private Password _passwordService = new Password();

        public LoginController(FoodOutletDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegister)
        {
            byte[] salt = _passwordService.createSalt(256);
            string saltBase64tring = Convert.ToBase64String(salt);

            var user = new User
            {
                //Mail = userRegister.Mail,
                Username = userRegister.Username,
                Password = userRegister.Password,
                Role = "User",
                Salt = saltBase64tring
            };
            user.Password = _passwordService.HashPassword(userRegister.Password, salt); // Hash the password before saving
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
                //new Claim(ClaimTypes.Email, user.Mail),
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
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
            if (user == null) return NotFound("login error: User not found");
            // Retrieve saved Salt for comparing hashes
            byte[] salt = getuserSaltDB(user);
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
        public async Task<IActionResult> getCurrentUser()
        {
            var claimCurrentUsername = User.FindFirst(ClaimTypes.Name);
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var claimCurrentUserRole = User.FindFirst(ClaimTypes.Role);

            var currentUsername = claimCurrentUsername?.Value;
            var currentUserIdstr = claimCurrentUserId?.Value;
            var currentUserRole = claimCurrentUserRole?.Value;

            if (currentUserIdstr == null || currentUsername == null || currentUserRole == null)
            {
                return BadRequest("Unauthenticated or user not found");
            }

            int currentUserId = int.Parse(claimCurrentUserId?.Value);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null)
            {
                return BadRequest("Unauthenticated or user not found");
            }

            var currentUserDTO = new UserDTO
            {
                Id = currentUserId,
                Username = currentUsername,
                Role = currentUserRole
            };
            return Ok(currentUserDTO);
        }

        [Authorize]
        [HttpPost ("refreshToken")]
        public async Task<IActionResult> RefreshAccessToken(IConfiguration config) 
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimCurrentUserId == null) { return Unauthorized("No user ID claim found."); }
            int parsedClaimId = int.Parse(claimCurrentUserId?.Value);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == parsedClaimId);
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
