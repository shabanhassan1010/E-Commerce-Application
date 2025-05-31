using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult<T> HandleResult<T>(T result)
        {
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        protected ActionResult<T> HandleResult<T>(T result, string message)
        {
            if (result == null)
                return NotFound(new ApiResponse(HttpStatusCode.NotFound, message));

            return Ok(new ApiResponse<T>(HttpStatusCode.OK, result, message));
        }

        protected ActionResult<T> HandleResult<T>(T result, HttpStatusCode statusCode, string message)
        {
            if (result == null)
                return NotFound(new ApiResponse(HttpStatusCode.NotFound, message));

            return StatusCode((int)statusCode, new ApiResponse<T>(statusCode, result, message));
        }

        protected ActionResult HandleResult(bool success, string message)
        {
            if (!success)
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, message));

            return Ok(new ApiResponse(HttpStatusCode.OK, message));
        }

        protected ActionResult HandleResult(bool success, HttpStatusCode statusCode, string message)
        {
            if (!success)
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, message));

            return StatusCode((int)statusCode, new ApiResponse(statusCode, message));
        }

        protected ActionResult HandleResult<T>(bool success, T data, string message)
        {
            if (!success)
                return BadRequest(new ApiResponse<T>(HttpStatusCode.BadRequest, data, message));

            return Ok(new ApiResponse<T>(HttpStatusCode.OK, data, message));
        }

        protected ActionResult HandleResult<T>(bool success, T data, HttpStatusCode statusCode, string message)
        {
            if (!success)
                return BadRequest(new ApiResponse<T>(HttpStatusCode.BadRequest, data, message));

            return StatusCode((int)statusCode, new ApiResponse<T>(statusCode, data, message));
        }
    }

    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(HttpStatusCode statusCode, string message)
        {
            StatusCode = (int)statusCode;
            Message = message;
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