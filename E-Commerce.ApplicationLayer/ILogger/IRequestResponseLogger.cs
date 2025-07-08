using Microsoft.AspNetCore.Http;

namespace E_Commerce.ApplicationLayer.ILogger
{
    public interface IRequestResponseLogger
    {
        Task<string> LogRequestAsync(HttpRequest request);
        Task LogResponseAsync(HttpResponse response, object responseBody);
    }
}
