using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public class APIResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public static APIResponse Success(string message = "Request was successful.")
        {
            return new APIResponse
            {
                IsSuccess = true,
                Message = message
            };
        }
        public static APIResponse Failure(List<string>? errors, string message = "Request failed.")
        {
            return new APIResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

    }
    public class APIResponse<T> : APIResponse
    {
        public T? Data { get; set; }
        public static APIResponse<T> Success(T data, string message = "Request was successful.")
        {
            return new APIResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }
        public static APIResponse<T> Failure(List<string>? errors, string message = "Request failed.")
        {
            return new APIResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

    }
}
