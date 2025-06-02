using FoodOutletRESTAPIDatabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using FoodOutletRESTAPIDatabase.Services.Logger;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

Logger.setDebugOutput(false); //whether to print debug lines to console or not

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
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new { error = "Authentication failed! Try again..." });
                    Logger.Log(Severity.ERROR, "Authentication failed: " + context.Exception.Message);
                    return context.Response.WriteAsync(result);
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Logger.Log(Severity.INFO, "Token Validated");
                return Task.CompletedTask;
            },
            OnMessageReceived = context => 
            {
                var token = context.Request.Cookies["Identity"];
                if (!string.IsNullOrEmpty(token)) {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context => 
            {
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new { error = "You are not authenticated!" });
                    return context.Response.WriteAsync(result);
                }
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new { error = "You don't have access to this content" });
                    Logger.Log(Severity.ERROR, "You lack the privileges to access this content");
                    return context.Response.WriteAsync(result);
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

});


// Cors testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Cors middleware
app.UseCors("AllowAll");

// Middleware for authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to routes
app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCookiePolicy();
app.Run();