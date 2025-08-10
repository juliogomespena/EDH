namespace EDH.Core.Common;

public readonly record struct Result<T>
{
    private Result(bool isSuccess, T? value, string[] errors) 
    {
        IsSuccess = isSuccess;
        _value = value;
        Errors = errors;
    }

    private Result(Exception exception)
    {
        IsSuccess = false;
        Exception = exception;
        Errors = ["Exception occurred."];
    }
    
    private readonly T? _value;
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value property on failed Result.");
    
    public bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public bool HasException => Exception is not null;

    public IReadOnlyList<string> Errors { get; }
    
    public Exception? Exception { get; }

    public static Result<T> Ok(T value) => value is null 
            ? Fail("Result.Ok received null value.") 
            : new Result<T>(true, value, []);

    public static Result<T> Fail(string error) => new(false, default, [error]);

    public static Result<T> Fail(params string[] errors) => new(false, default, errors is { Length: > 0 } 
        ? errors 
        : [ "Unknown error" ]);
    
    public static Result<T> Fail(Exception exception) => new(exception);
}

public readonly record struct Result
{
    private Result(bool isSuccess, string[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
    
    private Result(Exception? exception)
    {
        IsSuccess = false;
        Exception = exception;
        Errors = ["Exception occurred."];
    }
    
    public bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public bool HasException => Exception is not null;

    public IReadOnlyList<string> Errors { get; }
    
    public Exception? Exception { get; }

    public static Result Ok() => new(true, []);
    
    public static Result Fail(string error) => new(false, [error]);

    public static Result Fail(params string[] errors) => new(false, errors is { Length: > 0 } 
        ? errors 
        : [ "Unknown error" ]);
    
    public static Result Fail(Exception exception) => new(exception);
}