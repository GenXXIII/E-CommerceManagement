namespace ECommerce.Domain.Abstractions;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException();
        if (!isSuccess && error == null)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result<TValue> Failure<TValue>(string error) => new(default!, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    public TValue Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException();
            return _value;
        }
    }

    protected internal Result(TValue value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        _value = value;
    }
}
