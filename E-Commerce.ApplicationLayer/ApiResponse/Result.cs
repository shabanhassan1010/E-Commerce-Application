
namespace E_Commerce.ApplicationLayer.ApiResponse
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static Result<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static Result<T> Failure(string message) => new()
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
