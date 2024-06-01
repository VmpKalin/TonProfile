using Argentics.Backend.Profile.Core.DTOs;

namespace Argentics.Backend.Profile.Api.Extensions;

public static class ProfileCoreResponseExtensions
{
    public static string[]? CoreResponseErrorsToString(this ProfileCoreResponse r)
    {
        var errorsAsString = r.FailureReasons?
            .Select(x => x.ToString())
            .ToArray();

        return errorsAsString;
    }
}