using FoodOutletRESTAPIDatabase;
using FoodOutletRESTAPIDatabase.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//builder.Services.AddDbContext<FoodOutletDb>(opt => opt.UseInMemoryDatabase("FoodOutlet"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FoodOutletDb>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDirectoryBrowser();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("Authorization failed: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };

    });


//Cors testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddAuthorization();

var app = builder.Build();
//cors middleware
app.UseCors("AllowAll");

// Add middleware for authentication and authorization
app.UseAuthentication();
app.UseAuthorization();
//cors middleware
//app.UseCors("AllowAll");

// Map controllers to routes
app.MapControllers();

// Advanced Queries
//Sort by top rating
app.MapGet("/foodoutlets/top-rated", async (FoodOutletDb db) =>
await db.FoodOutlets
       .Select(fo => new
       {
           fo.Id,
           fo.Name,
           fo.Location,
           Rating = fo.Reviews.Any() ? Math.Round(fo.Reviews.Average(r => r.Score), 1) : 0,
           ReviewCount = fo.Reviews.Count
       })
       .OrderByDescending(fo => fo.Rating)
       //.Take(5) //only shows 5
       .ToListAsync());

//Get average review score by id of outlet
app.MapGet("/foodoutlets/{id}/average-rating", async (int id, FoodOutletDb db) =>
{
    var average = await db.Reviews.Where(r => r.FoodOutletId == id).AverageAsync(r => (double?)r.Score);
    return Results.Ok(average ?? 0);
});

//authoriaztion required posting review
app.MapPost("/foodoutlets/{foodOutletId}/reviews", async (int foodOutletId, Review review, FoodOutletDb db) =>
{
    if (review.Score < 1 || review.Score > 5)
    {
        return Results.BadRequest("Review score must be between 1 and 5");
    }
    else if (!await db.FoodOutlets.AnyAsync(fo => fo.Id == foodOutletId))
    {
        return Results.BadRequest("Food Outlet Not Found");
    }
    review.FoodOutletId = foodOutletId;
    db.Reviews.Add(review);
    await db.SaveChangesAsync();
    return Results.Created($"/foodoutlets/{foodOutletId}/reviews/{review.Id}", review);
}).RequireAuthorization();

app.MapGet("/reviews", async (FoodOutletDb db) =>
    await db.Reviews
        .Select(r => new
        {
            FoodOutlet = new
            {
                r.FoodOutlet.Id,
                r.FoodOutlet.Name,
                //r.FoodOutlet.Rating
            },
            r.Id,
            r.Comment,
            r.Score,
            r.CreatedAt
        })
        .ToListAsync());

app.MapGet("/foodoutlets/{foodOutletId}/reviews", async (int foodOutletId, FoodOutletDb db) =>
    await db.Reviews
        .Where(r => r.FoodOutletId == foodOutletId)
        .Select(r => new
        {
            FoodOutlet = new
            {
                r.FoodOutlet.Id,
                r.FoodOutlet.Name,
                //r.FoodOutlet.Rating
            },
            r.Id,
            r.Comment,
            r.Score,
            r.CreatedAt
        })
        .ToListAsync());

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();