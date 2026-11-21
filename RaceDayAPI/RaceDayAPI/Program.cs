using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using RaceDayAPI.Services; // Added for BlobService

namespace RaceDayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register Database Context
            builder.Services.AddDbContext<RaceDayDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                });

            // --- ADDED FOR PART 3: Register Blob Service ---
            builder.Services.AddScoped<BlobService>();

            // --- ADDED FOR PART 3: Configure CORS for MVC ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {

                    policy.WithOrigins("http://localhost:5227")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // CRITICAL: Allows cookies to pass through
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection(); // Keep commented out as per your setup

            // --- ADDED FOR PART 3: Use CORS ---
            // MUST be placed before UseAuthentication and UseAuthorization
            app.UseCors("AllowMVC");

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}