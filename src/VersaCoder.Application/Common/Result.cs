namespace VersaCoder.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    private Result(T? value, bool isSuccess, string? error = null, List<string>? errors = null)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T value) => new(value, true);
    public static Result<T> Failure(string error) => new(default, false, error);
    public static Result<T> Failure(List<string> errors) => new(default, false, errors: errors);
}
