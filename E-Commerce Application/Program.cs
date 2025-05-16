using E_Commerce.InfrastructureLayer.Data.DBContext;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen();

            #region DbContext Configuration
            //builder.Services.AddDbContext<ApplicationDBContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

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
