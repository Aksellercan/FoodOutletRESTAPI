using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FoodOutletRESTAPIDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodOutletController : ControllerBase
    {
        private readonly FoodOutletDb _db;

        public FoodOutletController(FoodOutletDb db)
        {
            _db = db;
        }

        //List all foodoutlets
        [HttpGet]
        public async Task<IActionResult> GetFoodOutlets()
        {
            var foodOutlets = await _db.FoodOutlets.Select(fo => new
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodOutletById(int id)
        {
            var foodOutlet = await _db.FoodOutlets.Where(fo => fo.Id == id).Select(fo => new
            {
            fo.Id,
            fo.Name,
            fo.Location,
            Rating = fo.Reviews.Any() ? Math.Round(fo.Reviews.Average(r => r.Score), 1) : 0,
            ReviewCount = fo.Reviews.Count
               }).FirstOrDefaultAsync();

            if (foodOutlet == null)
            {
                return NotFound();
            }
            return Ok(foodOutlet);
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> CreateFoodOutlet(FoodOutlet foodOutlet)
        {
            if (string.IsNullOrEmpty(foodOutlet.Name) || string.IsNullOrEmpty(foodOutlet.Location)) return BadRequest("Name and Location are required");
            _db.FoodOutlets.Add(foodOutlet);
            await _db.SaveChangesAsync();
            return Created($"/foodoutlets/{foodOutlet.Id}", foodOutlet);
        }

        //Update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFoodOutlet(int id, FoodOutlet updated)
        {
            var existing = await _db.FoodOutlets.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = updated.Name;
            existing.Location = updated.Location;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        //Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoodOutlet(int id)
        {
            var outlet = await _db.FoodOutlets.FindAsync(id);
            if (outlet == null) return NotFound();
            _db.FoodOutlets.Remove(outlet);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
