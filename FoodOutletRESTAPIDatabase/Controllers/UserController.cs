using System.Data;
using FoodOutletRESTAPIDatabase.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(FoodOutletDb db) : Controller
    {
        readonly Password _passwordService = new();

        int GetCurrentUserId()
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = claimCurrentUserId?.Value;
            return (currentUserId != null ? int.Parse(currentUserId) : throw new UnauthorizedAccessException("Unauthenticated or user not found"));
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return (user != null ? Ok(user.Username) : NotFound("User not found"));
        }

        [Authorize]
        [HttpDelete("remove/{currUser:int}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int currUser)
        {
            try
            {
                var removeUser = await db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
                int currentUserId = GetCurrentUserId();

                if (removeUser == null) { throw new ArgumentNullException(null, "Cannot remove. User does not exist!"); }
                if ((removeUser.Id != currentUserId)) return Unauthorized();

                db.Users.Remove(removeUser);
                await db.SaveChangesAsync();
                return NoContent();
            } catch (Exception e) {
                return BadRequest($"Failed to remove user with id {currUser}. {e.Message}");
            }
        }

        [Authorize (Roles = "User")]
        [HttpPut("namech/{currUser:int}")]
        public async Task<IActionResult> UpdateUsername([FromRoute] int currUser, string newUsername)
        {
            try
            {
                var updateUsername = await db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
                int currentUserId = GetCurrentUserId();

                if (updateUsername == null) { throw new ArgumentNullException(null, "Cannot update username. User does not exist!"); }
                if ((updateUsername.Id != currentUserId)) return Unauthorized();

                updateUsername.Username = newUsername;
                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e) {
                return BadRequest($"Failed to update username for user with id {currUser}. {e.Message}");
            }
        }

        // [Authorize]
        // [HttpPut("mailch/{currUser:int}")]
        // public async Task<IActionResult> UpdateUserMail([FromRoute] int currUser, string newMail)
        // {
        //     try
        //     {
        //         var updateUserMail = await db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
        //         if (updateUserMail == null) { throw new ArgumentNullException(null, "User does not exist!"); }
        //         updateUserMail.Username = newMail;
        //         await db.SaveChangesAsync();
        //         return Ok();
        //     }
        //     catch (Exception e)
        //     {
        //         logger.LogError("{}", e.Message);
        //         return BadRequest($"Failed to update user mail for user with id {currUser}. {e.Message}");
        //     }
        // }

        [Authorize]
        [HttpPut("passch/{currUser:int}")]
        public async Task<IActionResult> UpdateUserPassword([FromRoute] int currUser, string newPassword)
        {
            try
            {
                var updateUserPassword = await db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
                if (updateUserPassword == null) { throw new ArgumentNullException(null, "Cannot update password. User does not exist!"); }
                byte[] compareSalt = Convert.FromBase64String(updateUserPassword.Salt);
                string compareHashes = _passwordService.HashPassword(newPassword, compareSalt);
                if (string.Equals(updateUserPassword.Password, compareHashes))
                {
                    throw new DuplicateNameException("New Password is same as old one or malformed.");
                }
                byte[] newSalt = _passwordService.CreateSalt(256);
                string saltBase64String = Convert.ToBase64String(newSalt);
                string newHashedPassword = _passwordService.HashPassword(newPassword, newSalt);
                updateUserPassword.Password = newHashedPassword;
                updateUserPassword.Salt = saltBase64String;
                await db.SaveChangesAsync();
                return Ok();
            } catch (Exception e) {
                return BadRequest($"Failed to update password for user with id {currUser}. {e.Message}");
            }
        }
    }
}
