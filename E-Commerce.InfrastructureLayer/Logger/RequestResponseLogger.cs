using E_Commerce.ApplicationLayer.ILogger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace E_Commerce.InfrastructureLayer.Logger
{
    public class RequestResponseLogger : IRequestResponseLogger
    {
        #region
        private readonly ILogger<RequestResponseLogger> _logger;
        public RequestResponseLogger(ILogger<RequestResponseLogger> logger)
        {
            _logger = logger;
        }
        #endregion

        public async Task<string> LogRequestAsync(HttpRequest request)
        {
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            _logger.LogInformation("Request Body: {Body}", body);
            return body;
        }

        public  Task LogResponseAsync(HttpResponse response, object responseBody)
        {
            var json = JsonSerializer.Serialize(responseBody);
            _logger.LogInformation(" Response Body: {Body}", json);
            return Task.CompletedTask;
        }
    }
}
