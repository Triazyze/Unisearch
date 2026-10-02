namespace UniSearch.Api.Models.Dtos;

public sealed class ErrorResponse
{
    public ErrorResponse(int statusCode, string errorCode, string message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Message = message;
    }

    public int StatusCode { get; }

    public string ErrorCode { get; }

    public string Message { get; }
}
