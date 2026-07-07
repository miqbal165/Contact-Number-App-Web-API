namespace ContactNumberWebAPI.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];

    public static ApiResponse<T> Ok(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? []
        };
    }

    public static ApiResponse<T> FromServiceResult(ServiceResult<T> result)
    {
        return new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors
        };
    }
}