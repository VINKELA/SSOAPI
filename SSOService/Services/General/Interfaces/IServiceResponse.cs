﻿using SSOService.Models;

namespace SSOService.Services.General.Interfaces
{
    public interface IServiceResponse
    {
        Response<T> FailedResponse<T>(T data, string Message = null) where T : class;
        Response<T> SuccessResponse<T>(T data, string Message = null) where T : class;
        Response<object> ExceptionResponse();


    }
}
