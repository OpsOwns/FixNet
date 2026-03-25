namespace FixNet.Domain.Base;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Successful result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failed result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(string code, string message) => Failure(Error.New(code, message));

    public static Result Combine(params Result[] results)
        => results.FirstOrDefault(r => r.IsFailure) ?? Success();
}

public class Result<T> : Result
{
    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("Cannot access Value of a failed Result. Use Error instead.");

    protected internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, Error.None);

    public new static Result<T> Failure(Error error) => new(default, false, error);

    public static implicit operator Result<T>(Error error) => Failure(error);
    public static implicit operator Result<T>(T value) => Success(value);
}