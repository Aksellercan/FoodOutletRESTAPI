using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodOutletController(FoodOutletDb db) : ControllerBase
    {
        //List all foodOutlets
        [HttpGet]
        public async Task<IActionResult> GetFoodOutlets()
        {
            var foodOutlets = await db.FoodOutlets.Select(fo => new
            {
                fo.Id,
                fo.Name,
                fo.Location,
                Rating = fo.Reviews.Any() ? Math.Round(fo.Reviews.Average(r => r.Score), 1) : 0,
                ReviewCount = fo.Reviews.Count
            }).ToListAsync();
            return Ok(foodOutlets);
        }

        //All CRUD
        //Read
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFoodOutletById(int id)
        {
            var foodOutlet = await db.FoodOutlets.Where(fo => fo.Id == id).Select(fo => new
            {
            fo.Id,
            fo.Name,
            fo.Location,
            Rating = fo.Reviews.Any() ? Math.Round(fo.Reviews.Average(r => r.Score), 1) : 0,
            ReviewCount = fo.Reviews.Count
            }).FirstOrDefaultAsync();

            return (foodOutlet != null ? Ok(foodOutlet) : NotFound());
        }

        //Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFoodOutlet(FoodOutlet foodOutlet)
        {
            if (string.IsNullOrEmpty(foodOutlet.Name) || string.IsNullOrEmpty(foodOutlet.Location)) return BadRequest("Name and Location are required");
            db.FoodOutlets.Add(foodOutlet);
            await db.SaveChangesAsync();
            return Created($"/foodoutlets/{foodOutlet.Id}", foodOutlet);
        }

        //Update
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFoodOutlet(int id, FoodOutlet updated)
        {
            var existing = await db.FoodOutlets.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = updated.Name;
            existing.Location = updated.Location;
            await db.SaveChangesAsync();
            return NoContent();
        }

        //Delete
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoodOutlet(int id)
        {
            var outlet = await db.FoodOutlets.FindAsync(id);
            if (outlet == null) return NotFound();
            db.FoodOutlets.Remove(outlet);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> TopRated()
        {
            var toprated = await db.FoodOutlets.Select(fo => new
            {
                fo.Id,
                fo.Name,
                fo.Location,
                Rating = fo.Reviews.Any() ? Math.Round(fo.Reviews.Average(r => r.Score), 1) : 0,
                ReviewCount = fo.Reviews.Count
            }
            ).ToListAsync();

            return Ok(toprated);
        }

        [HttpGet("{id:int}/average-rating")]
        public async Task<IActionResult> GetAverageRating([FromRoute] int id)
        {
            var averageRating = await db.Reviews.Where(r => r.FoodOutletId == id).AverageAsync(r => (double?)r.Score);
            return Ok(averageRating);
        }
    }
}
