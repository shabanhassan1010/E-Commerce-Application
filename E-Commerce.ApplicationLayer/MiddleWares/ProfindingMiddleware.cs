
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace E_Commerce.ApplicationLayer.MiddleWares
{
    public class ProfindingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProfindingMiddleware> _logger;   // _logger : I use it to track problems and improve performance

        public ProfindingMiddleware(RequestDelegate next , ILogger<ProfindingMiddleware> logger)  // dh constraints which is required for Middleware 
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        ///  HttpContext : contain  all info about req and response
        ///  Task : is peocess which is work in somewhere in (Processor) not (Main Thread)
        /// </summary>
        /// <param name="context"></param>
        public async Task Invoke(HttpContext context)   // is required -> return Task
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await _next(context);   // im wait context beacause he contain all info about req and response
            stopWatch.Stop();
            _logger.LogInformation($"request`{context.Request.Path} took {stopWatch.ElapsedMilliseconds}Ms`");
            await _next(context);
        }
    }
}
