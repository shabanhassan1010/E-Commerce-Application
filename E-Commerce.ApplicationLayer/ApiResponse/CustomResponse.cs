using System.Net;

namespace E_Commerce.ApplicationLayer.ApiResponse
{
    /// <summary>
    /// Base response class for all API responses
    /// </summary>
    public class CustomResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public CustomResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Response class for successful operations with data
    /// </summary>
    public class CustomResponse<T> : CustomResponse
    {
        public T Data { get; set; }

        public CustomResponse(int statusCode, string message, T data) 
            : base(statusCode, message)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Response class for error cases
    /// </summary>
    public class CustomErrorResponse : CustomResponse
    {
        public List<string> Errors { get; set; }

        public CustomErrorResponse(int statusCode, string message, List<string>? errors = null) 
            : base(statusCode, message)
        {
            Errors = errors ?? new List<string>();
        }
    }
} 