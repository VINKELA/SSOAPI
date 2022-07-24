using Microsoft.AspNetCore.Http;
using SSOService.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SSOService.MiddleWares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ExceptionMessage = "An Error Occured Contact Admin";
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = Response<object>.Fail(ExceptionMessage);
                // unhandled error
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var result = JsonSerializer.Serialize(responseModel);
                Console.WriteLine(error);
                await response.WriteAsync(result);
            }
        }
    }
}