namespace UniversityApi.Common;

public class ReturnResult<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TraceId { get; set; }

    public static ReturnResult<T> Success(T result, string? traceId, int statusCode = StatusCodes.Status200OK) => new()
    {
        StatusCode = statusCode,
        IsSuccess = true,
        Result = result,
        TraceId = traceId
    };

    public static ReturnResult<T> Fail(int statusCode, string errorCode, string errorMessage, string? traceId) => new()
    {
        StatusCode = statusCode,
        IsSuccess = false,
        Result = default,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage,
        TraceId = traceId
    };
}
