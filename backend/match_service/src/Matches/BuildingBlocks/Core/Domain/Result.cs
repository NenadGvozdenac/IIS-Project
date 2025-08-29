namespace match_service.src.Matches.BuildingBlocks.Core.Domain;

public class Result<T> : Result
{
    public T Value { get; }

    public Result(T value, bool isSuccess, string error, int code) : base(isSuccess, error, code)
    {
        Value = value;
    }

    public Result(T value, bool isSuccess, string error, ResultCode code) : base(isSuccess, error, code)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty, 200);
    public new static Result<T> Failure(string error) => new(default!, false, error, 500);
    public new Result<T> WithCode(int code)
    {
        Code = Enum.TryParse<ResultCode>(code.ToString(), out var resultCode) ? resultCode : ResultCode.InternalServerError;
        return this;
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public ResultCode Code { get; set; }

    protected Result(bool isSuccess, string error, int code)
    {
        IsSuccess = isSuccess;
        Error = error;
        Code = Enum.TryParse<ResultCode>(code.ToString(), out var resultCode) ? resultCode : ResultCode.InternalServerError;
    }

    protected Result(bool isSuccess, string error, ResultCode code)
    {
        IsSuccess = isSuccess;
        Error = error;
        Code = code;
    }

    public static Result Success() => new(true, string.Empty, 200);
    public static Result Failure(string error) => new(false, error, 500);

    public Result WithCode(int code)
    {
        Code = Enum.TryParse<ResultCode>(code.ToString(), out var resultCode) ? resultCode : ResultCode.InternalServerError;
        return this;
    }
}