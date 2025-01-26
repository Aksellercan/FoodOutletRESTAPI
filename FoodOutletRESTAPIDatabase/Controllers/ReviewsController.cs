//using FoodOutletRESTAPIDatabase.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;

//namespace FoodOutletRESTAPIDatabase.Controllers
//{
//    [Authorize(Roles = "User")]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReviewsController : ControllerBase
//    {
//        private readonly FoodOutletDb _db;

//        public ReviewsController(FoodOutletDb db)
//        {
//            _db = db;
//        }

//        // List all reviews for a specific food outlet
//        [HttpGet("{foodOutletId}")]
//        public async Task<IActionResult> GetReviews(int foodOutletId)
//        {
//            var reviews = await _db.Reviews.Where(r => r.FoodOutletId == foodOutletId).ToListAsync();
//            return Ok(reviews);
//        }

//        [HttpPost("{foodOutletId}/reviews")]
//        [Authorize(Roles = "User")]
//        public async Task<IActionResult> PostReview(int foodOutletId, [FromBody] Review review)
//        {
//            Console.WriteLine($"Food Outlet ID: {foodOutletId}");

//            if (review.Score < 1 || review.Score > 5)
//            {
//                return BadRequest("Review score must be between 1 and 5");
//            }
//            else if (!await _db.FoodOutlets.AnyAsync(fo => fo.Id == foodOutletId))
//            {
//                return BadRequest("Food Outlet Not Found");
//            }
//            review.FoodOutletId = foodOutletId;
//            _db.Reviews.Add(review);
//            await _db.SaveChangesAsync();
//            return Created($"/foodoutlets/{foodOutletId}/reviews/{review.Id}", review);
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetReviews()
//        {
//            var reviews = await (_db.Reviews.Select(r => new
//            {
//                FoodOutlet = new
//                {
//                    //r.FoodOutlet.Id,
//                    //r.FoodOutlet.Name,
//                },
//                r.Id,
//                r.Comment,
//                r.Score,
//                r.CreatedAt
//            }))
//                .ToListAsync();
//            return Ok(reviews);
//        }

//        //        app.MapGet("/foodoutlets/{foodOutletId}/reviews", async(int foodOutletId, FoodOutletDb db) =>
//        //    await db.Reviews
//        //        .Where(r => r.FoodOutletId == foodOutletId)
//        //        .Select(r => new
//        //        {
//        //            FoodOutlet = new
//        //            {
//        //                r.FoodOutlet.Id,
//        //                r.FoodOutlet.Name,
//        //                //r.FoodOutlet.Rating
//        //            },
//        //            r.Id,
//        //            r.Comment,
//        //            r.Score,
//        //            r.CreatedAt
//        //        })
//        //        .ToListAsync());


//        ////Create
//        //app.MapPost("/foodoutlets/{foodOutletId}/reviews", async(int foodOutletId, Review review, FoodOutletDb db) =>
//        //{
//        //    if (review.Score< 1 || review.Score> 5)
//        //    {
//        //        return Results.BadRequest("Review score must be between 1 and 5");
//        //    } else if (!await db.FoodOutlets.AnyAsync(fo => fo.Id == foodOutletId)) {
//        //        return Results.BadRequest("Food Outlet Not Found");
//        //    }
//        //review.FoodOutletId = foodOutletId;
//        //db.Reviews.Add(review);
//        //await db.SaveChangesAsync();
//        //return Results.Created($"/foodoutlets/{foodOutletId}/reviews/{review.Id}", review);
//        //});

//        //// List all reviews
//        //app.MapGet("/reviews", async (FoodOutletDb db) =>
//        //    await db.Reviews
//        //        .Select(r => new
//        //        {
//        //            FoodOutlet = new
//        //            {
//        //                r.FoodOutlet.Id,
//        //                r.FoodOutlet.Name,
//        //                //r.FoodOutlet.Rating
//        //            },
//        //            r.Id,
//        //            r.Comment,
//        //            r.Score,
//        //            r.CreatedAt
//        //        })
//        //        .ToListAsync());
//    }
//}
