using System.Net;

namespace Argentics.Backend.Profile.Api.Models.Responses;

public class ApiResponse
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; set; }
    public string[]? Errors { get; init; }

    public static ApiResponse Success(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse {IsSuccess = true, StatusCode = statusCode, Errors = null};
    }

    public static ApiResponse Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, string[] errors = null!)
    {
        return new ApiResponse {IsSuccess = false, StatusCode = statusCode, Errors = errors};
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Body { get; init; }

    public static ApiResponse<T> Success(T? body, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T> {IsSuccess = true, StatusCode = statusCode, Body = body, Errors = null!};
    }

    public static ApiResponse<T> Fail(
        T? body,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string[] errors = null!)
    {
        return new ApiResponse<T> {IsSuccess = false, StatusCode = statusCode, Errors = errors, Body = body};
    }
}