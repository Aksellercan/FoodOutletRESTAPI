using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FoodOutletRESTAPIDatabase.DTOs;
using System.Security.Claims;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController(FoodOutletDb db) : ControllerBase
    {
        int GetCurrentUserId()
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = claimCurrentUserId?.Value;
            return currentUserId == null ? throw new UnauthorizedAccessException("Unauthenticated or user not found") : int.Parse(currentUserId);
        }

        public bool IsAdmin()
        {
            var claimCurrentUser = User.FindFirst(ClaimTypes.Role);
            string? role = claimCurrentUser?.Value;
            return role?.Equals("Admin") ?? false;
        }

        // List all reviews for a specific food outlet
        [HttpGet("{foodOutletId:int}")]
        public async Task<IActionResult> GetReviews(int foodOutletId)
        {
            var reviews = await db.Reviews.Where(r => foodOutletId == r.FoodOutletId).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost("{outletid:int}/reviews")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostReview([FromRoute] int outletid, Review review)
        {
            if (review.Score is < 1 or > 5) return BadRequest("Score must be greater than 0 and lower than 5");

            //Find the outlet object
            var foodOutlet = await db.FoodOutlets.FirstOrDefaultAsync(fo => fo.Id == outletid);
            if (foodOutlet == null) return BadRequest("Outlet doesn't exist");

            try
            {
                int currentUserId = GetCurrentUserId();
                review.FoodOutlet = foodOutlet;
                review.UserId = currentUserId;
                await db.Reviews.AddAsync(review);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Failed to add review. " + e.Message);
            }
            //Create DTO object to return
            var reviewDto = new ReviewDTO
            {
                Id = review.Id,
                FoodOutletId = review.FoodOutletId,
                Comment = review.Comment,
                Score = review.Score,
                CreatedAt = review.CreatedAt
            };
            return Created($"/foodoutlets/{outletid}/reviews/{review.Id}", reviewDto);
        }

        //List reviews from one user
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReviewsFromUser()
        {
            int currentUserId = GetCurrentUserId();
            var userReviews = await db.Reviews.Where(r => r.UserId == currentUserId).Select(r => new {
                FoodOutlet = new {
                    r.FoodOutletId,
                    r.FoodOutlet!.Name
                },
                r.Id,
                r.Comment,
                r.Score,
                r.CreatedAt
            }).ToListAsync();
            return Ok(userReviews);
        }

        //delete a review
        [HttpDelete ("delete/review/{reviewId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteReviews([FromRoute] int reviewId)
        {
            int currentUserId = GetCurrentUserId();

            var reviewDelete = await db.Reviews.FindAsync(reviewId);

            if (reviewDelete == null) return NotFound("Review does not exist!");
            if ((reviewDelete.UserId != currentUserId) && !IsAdmin()) return Unauthorized(); //only current user and admin can delete reviews
            try {
                db.Reviews.Remove(reviewDelete);
                await db.SaveChangesAsync();
                return NoContent();
            } catch (Exception e) {
                return BadRequest($"Error deleting review for r.id {reviewId}. Exception: {e.Message}");
            }
        }
    }
}