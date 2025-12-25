namespace odev.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        
        // Helper for Controller to decide Status Code (not serialized to client usually, but here we can keep it hidden or public)
        // Check requirement: Format must be { success, message, data }. StatusCode shouldn't be in JSON potentially. 
        // We can use [JsonIgnore] if we want to hide it, but let's just use it as a helper.
        [System.Text.Json.Serialization.JsonIgnore]
        public int StatusCode { get; set; } = 200;

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data, StatusCode = 200 };
        }

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
        {
            return new ApiResponse<T> { Success = false, Message = message, Data = default, StatusCode = statusCode };
        }
    }
}
