using E_Commerce.DomainLayer;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using E_Commerce.InfrastructureLayer.Data.GenericClass;
using E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion;
using E_Commerce.ApplicationLayer.MiddleWares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;

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

            #region Dependency Injection
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            #endregion

            builder.Services.AddCors();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Add Exception Middleware
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
