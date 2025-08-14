namespace movielogger.services.models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public ErrorType ErrorType { get; }

    private Result(bool isSuccess, T? value, string? errorMessage, ErrorType errorType = ErrorType.None)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string errorMessage, ErrorType errorType = ErrorType.Validation) => new(false, default, errorMessage, errorType);
    public static Result<T> NotFound(string errorMessage) => new(false, default, errorMessage, ErrorType.NotFound);
    public static Result<T> Unauthorized(string errorMessage) => new(false, default, errorMessage, ErrorType.Unauthorized);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public ErrorType ErrorType { get; }

    private Result(bool isSuccess, string? errorMessage, ErrorType errorType = ErrorType.None)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string errorMessage, ErrorType errorType = ErrorType.Validation) => new(false, errorMessage, errorType);
    public static Result NotFound(string errorMessage) => new(false, errorMessage, ErrorType.NotFound);
    public static Result Unauthorized(string errorMessage) => new(false, errorMessage, ErrorType.Unauthorized);
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Unauthorized,
    Conflict,
    Internal
}