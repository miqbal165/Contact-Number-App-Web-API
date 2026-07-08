using System.Text.Json.Serialization;

namespace ContactNumberWebAPI.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> FromServiceResult(ServiceResult<T> result)
    {
        return new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };
    }

    public static ApiResponse<T> ValidationFailure(
        IReadOnlyList<string> errors,
        string message = "Validasi data gagal.")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }

    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}