using System.Net;

namespace E_Commerce.ApplicationLayer.ApiResponse
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiErrorResponse(int statusCode, string message, string? details = null)  // Constructor
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiErrorResponse NotFound(string message = "Resource not found")
        {
            return new ApiErrorResponse((int)HttpStatusCode.NotFound,message);
        }

        public static ApiErrorResponse BadRequest(string message = "Invalid request")
        {
            return new ApiErrorResponse((int)HttpStatusCode.BadRequest,message );
        }

        public static ApiErrorResponse Unauthorized(string message = "Unauthorized access")
        {
            return new ApiErrorResponse((int)HttpStatusCode.Unauthorized, message );
        }

        public static ApiErrorResponse Forbidden(string message = "Access forbidden")
        {
            return new ApiErrorResponse((int)HttpStatusCode.Forbidden,message);
        }

        public static ApiErrorResponse InternalServerError(string message = "An error occurred while processing your request", string? details = null)
        {
            return new ApiErrorResponse( (int)HttpStatusCode.InternalServerError,message,details);
        }

        public static ApiErrorResponse ValidationError(string message = "Validation failed", string? details = null)
        {
            return new ApiErrorResponse((int)HttpStatusCode.BadRequest,message,details);
        }
    }
}
