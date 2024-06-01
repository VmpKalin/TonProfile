namespace Argentics.Backend.Profile.Core.DTOs;

public class ProfileCoreResponse
{
    protected ProfileCoreResponse()
    {
    }
    
    public bool IsSuccess { get; set; }
    public FailureReason[]? FailureReasons { get; set; }

    public static ProfileCoreResponse Success()
    {
        return new ProfileCoreResponse {IsSuccess = true, FailureReasons = null};
    }

    public static ProfileCoreResponse Fail(FailureReason[]? failureReasons = null)
    {
        return new ProfileCoreResponse {IsSuccess = false, FailureReasons = failureReasons};
    }
}

public class ProfileCoreResponse<T> : ProfileCoreResponse
{
    protected ProfileCoreResponse()
    {
    }

    public T? Body { get; set; }
    
    public static ProfileCoreResponse<T> Success(T? body)
    {
        return new ProfileCoreResponse<T> {IsSuccess = true, Body  = body, FailureReasons = null};
    }

    public static ProfileCoreResponse<T> Fail(T?body, FailureReason[]? failureReasons = null)
    {
        return new ProfileCoreResponse<T> {IsSuccess = false, Body = body, FailureReasons = failureReasons};
    }
}

public enum FailureReason
{
    MongoDbError,
    DateInconsistency,
    NotFound
}