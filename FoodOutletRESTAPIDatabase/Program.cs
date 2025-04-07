using FoodOutletRESTAPIDatabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FoodOutletDb>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
                //context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { error = "Authentication failed! Try again..." });
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return context.Response.WriteAsync(result);
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token Validated");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { error = "You  don't have access to this content" });
                Console.WriteLine("You lack the privilages to access this content");
                return context.Response.WriteAsync(result);
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
       .ToListAsync());

//Get average review score by id of outlet
app.MapGet("/foodoutlets/{id}/average-rating", async (int id, FoodOutletDb db) =>
{
    var average = await db.Reviews.Where(r => r.FoodOutletId == id).AverageAsync(r => (double?)r.Score);
    return Results.Ok(average ?? 0);
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();