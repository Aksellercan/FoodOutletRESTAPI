using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FoodOutletRESTAPIDatabase.DTOs;

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
            Console.WriteLine("START PROCESS HERE");
            Console.WriteLine($"Food Outlet ID detected from URL {outletid}");
            if (review.Score < 1 || review.Score > 5) return BadRequest("Score must be greater than 0 and lower than 5");

            //Find the outlet object
            var foodOutletretrieved = await _db.FoodOutlets.FirstOrDefaultAsync(fo => fo.Id == outletid);

            if (foodOutletretrieved == null) return BadRequest("Outlet doesn't exist");

            try
            {
                review.FoodOutlet = foodOutletretrieved;

                Console.WriteLine("Trying to add review for " + review.FoodOutlet.Id);
                await _db.Reviews.AddAsync(review);
                Console.WriteLine("SAVING...");
                await _db.SaveChangesAsync();
                Console.WriteLine("FINISHED SUCCESFULLY!");
            }
            catch (Exception e) { Console.WriteLine(e + "Failed to add review..."); } //dont forget to change this ;)

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
        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await (_db.Reviews.Select(r => new
            {
                FoodOutlet = new
                {
                    //r.FoodOutlet.Id,
                    //r.FoodOutlet.Name,
                },
                r.Id,
                r.Comment,
                r.Score,
                r.CreatedAt
            }))
                .ToListAsync();
            return Ok(reviews);
        }
    }
}
