namespace Shared.Domain.Core;

/// <summary>
/// Generic result wrapper for operation outcomes
/// </summary>
public class Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public string Message { get; }
    public string? ErrorCode { get; }
    public List<string> Errors { get; }

    public Result(bool success, T? data, string message, string? errorCode = null, List<string>? errors = null)
    {
        Success = success;
        Data = data;
        Message = message;
        ErrorCode = errorCode;
        Errors = errors ?? new();
    }

    public static Result<T> Ok(T data, string message = "Success")
        => new(true, data, message);

    public static Result<T> Failure(string message, string? errorCode = null, List<string>? errors = null)
        => new(false, default, message, errorCode, errors);
}

public class Result
{
    public bool Success { get; }
    public string Message { get; }
    public string? ErrorCode { get; }
    public List<string> Errors { get; }

    public Result(bool success, string message, string? errorCode = null, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        ErrorCode = errorCode;
        Errors = errors ?? new();
    }

    public static Result Ok(string message = "Success")
        => new(true, message);

    public static Result Failure(string message, string? errorCode = null, List<string>? errors = null)
        => new(false, message, errorCode, errors);
}