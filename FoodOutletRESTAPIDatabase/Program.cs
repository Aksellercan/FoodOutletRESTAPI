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

        //TODO: custom jwt messages causing middleware issues
        /*
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
            }
            ,
            OnForbidden = context =>
            {
                //context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new { error = "You  don't have access to this content" });
                Console.WriteLine("You lack the privileges to access this content");
                return context.Response.WriteAsync(result);
            }
        };
        */
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

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();