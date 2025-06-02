using FoodOutletRESTAPIDatabase.Services.Logger;
using FoodOutletRESTAPIDatabase.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly FoodOutletDb _db;
        private Password _passwordService = new Password();

        public UserController(FoodOutletDb db) 
        {
            _db = db;
        }

        private int getCurrentUserId()
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = claimCurrentUserId?.Value;
            if (currentUserId == null)
            {
                throw new UnauthorizedAccessException("Unauthenticated or user not found");
            }
            return int.Parse(currentUserId);
        }

        [Authorize]
        [HttpDelete("remove/{currUser}")]
        public async Task<IActionResult> removeUser([FromRoute] int currUser)
        {
            var removeUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
            int currentUserId = getCurrentUserId();

            if (removeUser == null) { return BadRequest("Cannot remove. User does not exist!"); }
            if ((removeUser.Id != currentUserId)) return Unauthorized();

            try
            {
                _db.Users.Remove(removeUser);
                await _db.SaveChangesAsync();
                Logger.Log(Severity.DEBUG, $"User with id {removeUser.Id} is successfully removed");
            } catch (Exception e) {
                return BadRequest($"Failed to remove user with id {currUser}. {e.Message}");
            }
            return NoContent();
        }

        [Authorize (Roles = "User")]
        [HttpPut("namech/{currUser}")]
        public async Task<IActionResult> updateUsername([FromRoute] int currUser, string newUsername)
        {
            var updateUsername = await _db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
            int currentUserId = getCurrentUserId();

            if (updateUsername == null) { return BadRequest("Cannot remove. User does not exist!"); }
            if ((updateUsername.Id != currentUserId)) return Unauthorized();

            try 
            {
                updateUsername.Username = newUsername;
                await _db.SaveChangesAsync();
                Logger.Log(Severity.DEBUG, $"Username updated to {updateUsername.Username}");
            }
            catch (Exception e) {
                return BadRequest($"Failed to update username for user with id {currUser}. {e.Message}");
            }
            return Ok();
        }

        //[Authorize]
        //[HttpPut("mailch/[currUser]")]
        //public async Task<IActionResult> updateUsermail([FromRoute] int currUser, string newMail)
        //{
        //    var updateUsermail = await _db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
        //    if (updateUsermail == null) { return BadRequest("Cannot remove. User does not exist!"); }

        //    try
        //    {
        //        updateUsermail.Username = newMail;
        //        await _db.SaveChangesAsync();
        //        Console.WriteLine($"Username updated to {updateUsermail.Username}");
        //    } catch (Exception e) { }
        //    return Ok();
        //}

        [Authorize]
        [HttpPut("passch/{currUser}")]
        public async Task<IActionResult> updateUserPassword([FromRoute] int currUser, string newPassword)
        {
            var updateUserpassword = await _db.Users.FirstOrDefaultAsync(u => u.Id == currUser);
            if (updateUserpassword == null) { return BadRequest("Cannot remove. User does not exist!"); }

            try 
            {
                byte[] compareSalt = Convert.FromBase64String(updateUserpassword.Salt);
                string compareHashes = _passwordService.HashPassword(newPassword, compareSalt);
                if (string.Equals(updateUserpassword.Password, compareHashes))
                {
                    Logger.Log(Severity.WARN, "New Password is same as old one or malformed.");
                    return BadRequest("New Password is same as old one or malformed.");
                }
                byte[] newSalt = _passwordService.createSalt(256);
                string saltBase64tring = Convert.ToBase64String(newSalt);
                string newHashedPassword = _passwordService.HashPassword(newPassword, newSalt);
                updateUserpassword.Password = newHashedPassword;
                updateUserpassword.Salt = saltBase64tring;
                await _db.SaveChangesAsync();
            } catch (Exception e) {
                return BadRequest($"Failed to update password for user with id {currUser}. {e.Message}");
            }
            return Ok();
        }
    }
}
