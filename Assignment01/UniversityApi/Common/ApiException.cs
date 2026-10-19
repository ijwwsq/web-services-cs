namespace UniversityApi.Common;

public class ApiException : Exception
{
    public ApiException(int statusCode, string errorCode, string message) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public int StatusCode { get; }
    public string ErrorCode { get; }

    public static ApiException NotFound(string errorCode, string message) =>
        new(StatusCodes.Status404NotFound, errorCode, message);

    public static ApiException Conflict(string errorCode, string message) =>
        new(StatusCodes.Status409Conflict, errorCode, message);

    public static ApiException BadRequest(string errorCode, string message) =>
        new(StatusCodes.Status400BadRequest, errorCode, message);
}
