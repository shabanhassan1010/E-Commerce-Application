﻿using E_Commerce.DomainLayer.Comman;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Text.Json;
namespace E_Commerce.ApplicationLayer.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly IHostEnvironment _hostEnvironment;  // Represents the hosting environment
        private readonly RequestDelegate _next; // Represents the next middleware in the pipeline
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(IHostEnvironment hostEnvironment, RequestDelegate next , ILogger<ExceptionMiddleware> logger)
        {
            _hostEnvironment = hostEnvironment;
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// This is the main middleware method that gets called for every request
        /// If any exception occurs anywhere in your application, it will be caught here
        /// and handled by the HandleExceptionAsync method
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // move from middlware to anther and if i found any error
            }
            catch (Exception ex)    // i will go back and throw Exception
            {
                _logger.LogError(ex,ex.Message,context.Request, "Unhandled Exception Occurred!");
                await HandleExceptionAsync(context, ex);
            }
         }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json"; // set the content type to json
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // set the status code to 500
            var response = _hostEnvironment.IsDevelopment()  // if i am in development mode will Includes the error message and stack trace
                ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiErrorResponse(context.Response.StatusCode, "Internal Server Error");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));  // Serializes the response to JSON and sends it back to the client
        }

        /// <summary>
        /// [1]   When a request comes to your API:
        ///       Request → ExceptionMiddleware → Your Controllers → Response  
        /// [2]   If everything works fine:
        ///       Request → ExceptionMiddleware → Your Controllers → Response
        /// [3]   If an error occurs:
        ///       Request → ExceptionMiddleware → [Error Occurs] → HandleExceptionAsync → Error Response
        /// </summary>

    }
}
