namespace ContactNumberWebAPI.Common;

public class ServiceResult<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = [];

    public ServiceResultStatus Status { get; init; }

    public static ServiceResult<T> Ok(T data, string message = "Success")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Status = ServiceResultStatus.Success
        };
    }

    public static ServiceResult<T> Created(T data, string message = "Created")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Status = ServiceResultStatus.Created
        };
    }

    public static ServiceResult<T> Fail(
        string message,
        ServiceResultStatus status = ServiceResultStatus.BadRequest,
        IReadOnlyList<string>? errors = null)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? [],
            Status = status
        };
    }
}