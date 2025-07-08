using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace E_Commerce.ApplicationLayer.MiddleWares
{
    public class RequestResponseLoggingMiddleware : IMiddleware
    {
        #region Constructor
        private readonly ILogger<RequestResponseLoggingMiddleware> logger;
        public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger)
        {
            this.logger = logger;
        }
        #endregion
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            // Read Request body and after read it the body is logged
            var request = await ReadBodyFromRequest(httpContext.Request);
            logger.LogInformation("Request: {request}", request);

            // Intercept the Response Body
            var originalResponseBody = httpContext.Response.Body;
            using var newResponseBody = new MemoryStream();
            httpContext.Response.Body = newResponseBody;

            // Call the next middleware in the pipeline
            await next(httpContext);

            // Read and Log the Response Body
            newResponseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            logger.LogInformation("Response: {response}", responseBodyText);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        private static async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
            request.EnableBuffering();

            // Uses a StreamReader to read the body stream as text.
            // leaveOpen: true: -> keeps the stream open so the next middleware can still access it.
            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();

            // Reset the request's body stream position for next middleware in the pipeline.
            request.Body.Position = 0;
            return requestBody;
        }
    }
}
