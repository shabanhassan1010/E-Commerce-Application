using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DomainLayer.ApiResponse
{
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

        public ApiResponse(HttpStatusCode statusCode, T data, string message) : base( statusCode , message)
        {
            Data = data;
        }
    }
}
