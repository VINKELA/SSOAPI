using SSOService.Models;
using SSOService.Services.General.Interfaces;

namespace SSOService.Services.Repositories.NonRelational.Implementations
{
    public class ServiceReponse : IServiceResponse
    {
        private const string ExceptionMessage = "An Error Occured, contact Admin";
        private const string SuccessMessage = "Success";

        public Response<T> FailedResponse<T>(T data, string message = null) where T : class
        {
            return Response(data, message ?? ExceptionMessage, false);
        }

        public Response<T> SuccessResponse<T>(T data, string message = null) where T : class
        {

            return Response(data, message ?? SuccessMessage, true);
        }
        public Response<object> ExceptionResponse()
        {

            return Response(new object(), ExceptionMessage, false);
        }

        private static Response<T> Response<T>(T data, string message, bool status) where T : class
        {

            return new Response<T>()
            {
                Data = data,
                Message = message,
                Status = status
            };
        }

    }
}
