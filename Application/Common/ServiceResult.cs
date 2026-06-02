namespace StudentProjectManagementSystem.Application.Common;

public class ServiceResult<T>
{
    public bool Success => StatusCode >= 200 && StatusCode < 300;

    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public static ServiceResult<T> Ok(string message, T? data)
    {
        return new ServiceResult<T> { Message = message, StatusCode = 200, Data = data };
    }

    public static ServiceResult<T> BadRequest(string message)
    {
        return new ServiceResult<T> { Message = message, StatusCode = 400, Data = default };
    }

    public static ServiceResult<T> NotFound(string message)
    {
        return new ServiceResult<T> { Message = message, StatusCode = 404, Data = default };
    }

    public static ServiceResult<T> Conflict(string message)
    {
        return new ServiceResult<T> { Message = message, StatusCode = 409, Data = default };
    }
}
