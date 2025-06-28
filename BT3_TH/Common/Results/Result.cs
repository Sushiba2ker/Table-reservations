namespace BT3_TH.Common.Results;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string ErrorMessage { get; protected set; } = string.Empty;
    public int StatusCode { get; protected set; } = 200;

    protected Result(bool isSuccess, string errorMessage, int statusCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage ?? string.Empty;
        StatusCode = statusCode;
    }

    public static Result Success(int statusCode = 200)
    {
        return new Result(true, string.Empty, statusCode);
    }

    public static Result Failure(string errorMessage, int statusCode = 400)
    {
        return new Result(false, errorMessage, statusCode);
    }

    public static Result NotFound(string errorMessage = "Resource not found")
    {
        return new Result(false, errorMessage, 404);
    }

    public static Result Unauthorized(string errorMessage = "Unauthorized access")
    {
        return new Result(false, errorMessage, 401);
    }

    public static Result ValidationError(string errorMessage)
    {
        return new Result(false, errorMessage, 422);
    }
}

/// <summary>
/// Represents the result of an operation that can succeed or fail and returns data
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data, string errorMessage, int statusCode)
        : base(isSuccess, errorMessage, statusCode)
    {
        Data = data;
    }

    public static Result<T> Success(T data, int statusCode = 200)
    {
        return new Result<T>(true, data, string.Empty, statusCode);
    }

    public static new Result<T> Failure(string errorMessage, int statusCode = 400)
    {
        return new Result<T>(false, default, errorMessage, statusCode);
    }

    public static new Result<T> NotFound(string errorMessage = "Resource not found")
    {
        return new Result<T>(false, default, errorMessage, 404);
    }

    public static new Result<T> Unauthorized(string errorMessage = "Unauthorized access")
    {
        return new Result<T>(false, default, errorMessage, 401);
    }

    public static new Result<T> ValidationError(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage, 422);
    }
}
