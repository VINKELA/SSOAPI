using System.Text.Json.Serialization;

namespace SSOService.Models
{
    public class Response<T> where T : class
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        public static Response<T> Fail(string errorMessage)
        {
            return new Response<T> { Status = false, Message = errorMessage };
        }
        public static Response<T> Success(T data, string message = "success")
        {
            return new Response<T> { Status = true, Data = data, Message = message };
        }

    }
}
