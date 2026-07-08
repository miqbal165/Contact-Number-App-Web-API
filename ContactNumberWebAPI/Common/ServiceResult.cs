namespace ContactNumberWebAPI.Common;

public class ServiceResult<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public ServiceResultStatus Status { get; init; }

    public static ServiceResult<T> Ok(
        T data,
        string message = "Berhasil.")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Status = ServiceResultStatus.Success
        };
    }

    public static ServiceResult<T> Created(
        T data,
        string message = "Data berhasil dibuat.")
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
        ServiceResultStatus status = ServiceResultStatus.BadRequest)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Status = status
        };
    }
}