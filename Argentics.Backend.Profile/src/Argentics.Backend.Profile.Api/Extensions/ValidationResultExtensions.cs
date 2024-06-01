using FluentValidation.Results;

namespace Argentics.Backend.Profile.Api.Extensions;

public static class ValidationResultExtensions
{
    public static string[] GetValidationErrorsAsArray(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
    }
}
