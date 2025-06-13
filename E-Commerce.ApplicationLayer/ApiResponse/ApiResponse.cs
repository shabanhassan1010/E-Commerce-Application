using System.Net;

namespace E_Commerce.ApplicationLayer.ApiResponse
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse(HttpStatusCode statusCode, string message)
        {
            StatusCode = (int)statusCode;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse(HttpStatusCode statusCode, T data, string message) 
            : base(statusCode, message)
        {
            Data = data;
        }
    }
} 