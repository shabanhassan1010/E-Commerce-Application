
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace E_Commerce.ApplicationLayer.MiddleWares
{
    public class RateLimitingMiddleware
    {
        /// <summary>
        /// Middlewares ----> is work or use Scoped dependency Injection
        /// </summary>

        public static int _counter = 0;
        public static DateTime _LastrequestDate = DateTime.UtcNow;
        private readonly RequestDelegate _next;
        public RateLimitingMiddleware(RequestDelegate next )
        {
            _next = next;
        }

        public async Task invoke (HttpContext context)
        {
            _counter ++;
            if(DateTime.Now.Subtract(_LastrequestDate).Seconds > 10)
            {
                _counter = 1;
                _LastrequestDate = DateTime.UtcNow;
                await _next(context);
            }
            else
            {
                if(_counter > 5)
                {
                    _LastrequestDate = DateTime.Now;
                    await context.Response.WriteAsync("Rate Limit Exceeded");
                }
                else
                {
                    _LastrequestDate = DateTime.Now;
                    await _next(context);
                }
            }
        }
    }
}
