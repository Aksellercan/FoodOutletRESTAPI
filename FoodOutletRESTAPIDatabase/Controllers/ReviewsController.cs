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

        public int getCurrentUserId()
        {
            var claimCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserId = claimCurrentUserId?.Value;
            if (currentUserId == null)
            {
                return 0;
            }
            return int.Parse(currentUserId);
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
            Console.WriteLine($"Food Outlet ID detected from URL {outletid}");
            if (review.Score < 1 || review.Score > 5) return BadRequest("Score must be greater than 0 and lower than 5");

            //Find the outlet object
            var foodOutletretrieved = await _db.FoodOutlets.FirstOrDefaultAsync(fo => fo.Id == outletid);

            if (foodOutletretrieved == null) return BadRequest("Outlet doesn't exist");

            int curentUserId = getCurrentUserId();
            if (curentUserId == 0) return BadRequest("User not found");

            try
            {
                review.FoodOutlet = foodOutletretrieved;
                //review.UserId = curentUserId;
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

        //To Add later: list reviews from one user
        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetReviews()
        //{
        //   var userReviews = await _db.Reviews.Where(r => r.UserId == currentUserId).ToListAsync();

        //    var reviews = userReviews.Select(r => new
        //    {
        //        FoodOutlet = new
        //        {
        //            //r.FoodOutlet.Id,
        //            //r.FoodOutlet.Name,
        //        },
        //        r.Id,
        //        r.Comment,
        //        r.Score,
        //        r.CreatedAt
        //    }).ToList();

        //    return Ok(reviews);
        //}
    }
}
