#region
using E_Commerce.ApplicationLayer.ActionFilters;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.ApplicationLayer.MiddleWares;
using E_Commerce.ApplicationLayer.Service;
using E_Commerce.DomainLayer;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using E_Commerce.InfrastructureLayer.Data.GenericClass;
using E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
#endregion

namespace E_Commerce_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen();

            #region DbContext Configuration
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion


            #region Redis Cache Configuration
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "ECommerce_";
            });
            #endregion


            #region Identity Configuration  -> ( Must be before JWT Authentication)
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();
            #endregion


            #region JWT Authentication Configuration

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Why should Authenticate you
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // tell him what he will do if req not Authenticate
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion


            #region Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim(ClaimTypes.Role, AppRole.Admin.ToString()));
                options.AddPolicy("CustomerPolicy", policy =>
                    policy.RequireClaim(ClaimTypes.Role, AppRole.customer.ToString()));
            });
            #endregion


            #region Dependency Injection
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            #endregion


            #region Action Filters Registration
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LogSensitiveActionAttribute>(); // Register global filter
            });
            #endregion

            //builder.Services.AddCors();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            #region Middleware Pipeline Configuration
            // Exception handling should be first in the pipeline
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            #endregion
        }
    }
}
