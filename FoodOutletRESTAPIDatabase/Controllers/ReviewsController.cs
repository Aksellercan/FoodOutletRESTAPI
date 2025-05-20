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
    public class ReviewsController : ControllerBase
    {
        private readonly FoodOutletDb _db;

        public ReviewsController(FoodOutletDb db)
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

        public bool isAdmin()
        {
            var claimCurrentUser = User.FindFirst(ClaimTypes.Role);
            string role = claimCurrentUser?.Value.ToString();
            Console.WriteLine("Current role of user is " + role);
            if (role.Equals("Admin"))
            {
                return true;
            }
            Console.WriteLine("Admin Role Verified!");
            return false;
        }

        // List all reviews for a specific food outlet
        [HttpGet("{foodOutletId}")]
        public async Task<IActionResult> GetReviews(int foodOutletId)
        {
            var reviews = await _db.Reviews.Where(r => foodOutletId == r.FoodOutletId).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost("{outletid}/reviews")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostReview([FromRoute] int outletid, Review review)
        {
            if (review.Score < 1 || review.Score > 5) return BadRequest("Score must be greater than 0 and lower than 5");

            //Find the outlet object
            var foodOutletretrieved = await _db.FoodOutlets.FirstOrDefaultAsync(fo => fo.Id == outletid);
            if (foodOutletretrieved == null) return BadRequest("Outlet doesn't exist");

            try
            {
                int curentUserId = getCurrentUserId();
                review.FoodOutlet = foodOutletretrieved;
                review.UserId = curentUserId;
                await _db.Reviews.AddAsync(review);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Failed to add review. " + e.Message);
            }
            //Create DTO object to return
            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                FoodoutletId = review.FoodOutletId,
                Comment = review.Comment,
                Score = review.Score,
                CreatedAt = review.CreatedAt
            };
            return Created($"/foodoutlets/{outletid}/reviews/{review.Id}", reviewDTO);
        }

        //List reviews from one user
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReviewsfromUser()
        {
            var userReviews = await _db.Reviews.Where(r => r.UserId == getCurrentUserId()).Select(r => new {
                FoodOutlet = new {
                    r.FoodOutletId,
                    r.FoodOutlet.Name
                },
                r.Id,
                r.Comment,
                r.Score,
                r.CreatedAt
            }).ToListAsync();

            Console.WriteLine(userReviews.Count);
            return Ok(userReviews);
        }

        //delete a review
        [HttpDelete ("delete/review/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> deleteReviews([FromRoute] int reviewId) 
        {
            bool test = isAdmin();
            Console.WriteLine(test);
            int currentUserId = getCurrentUserId();
            

            var reviewDelete = await _db.Reviews.FindAsync(reviewId);

            if (reviewDelete == null) return NotFound("Review does not exist!");
            if (reviewDelete.UserId != currentUserId && !isAdmin()) return Unauthorized(); //only current user and admin can delete reviews
            try {
                _db.Reviews.Remove(reviewDelete);
                await _db.SaveChangesAsync();
                return NoContent();
            } catch (Exception e) {
                return BadRequest($"Error deleting review for r.id {reviewId}. Exception: {e.Message}");
            }
        }
    }
}