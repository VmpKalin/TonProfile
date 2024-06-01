using Argentics.Backend.Profile.Api.Models.InternalRequests;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Api.Validators;

public class SaveProfileValidator : AbstractValidator<AddOrUpdateProfileRequest>
{
    public SaveProfileValidator()
    {
        RuleFor(x => x.ProfileJson)
            .NotEmpty()
            .Must(BelongTo).WithMessage("User can update only his profile");
    }

    protected override bool PreValidate(ValidationContext<AddOrUpdateProfileRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate is not null)
        {
            return true;
        }

        result.Errors.Add(new ValidationFailure("", "Instance to validate must be non null"));
        return false;
    }

    private bool BelongTo(AddOrUpdateProfileRequest addOrUpdateProfileRequest, JObject json)
    {
        try
        {
            var userIdFromProfile = json["user_id"];
            if (userIdFromProfile is null)
            {
                return false;
            }

            return addOrUpdateProfileRequest.UserId == userIdFromProfile.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}