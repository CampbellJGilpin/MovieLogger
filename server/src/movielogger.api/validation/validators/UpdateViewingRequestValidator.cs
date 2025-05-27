using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.viewings;

namespace movielogger.api.validation.validators;

public class UpdateViewingRequestValidator : AbstractValidator<UpdateViewingRequest>
{
    public UpdateViewingRequestValidator()
    {
        RuleFor(x => x.DateViewed).NotEmpty().WithMessage("DateViewed is required");
    }
}