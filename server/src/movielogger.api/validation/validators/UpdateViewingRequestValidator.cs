using FluentValidation;
using movielogger.api.models.requests.viewings;

namespace movielogger.api.validation.validators;

public class UpdateViewingRequestValidator : AbstractValidator<UpdateViewingRequest>
{
    public UpdateViewingRequestValidator()
    {
        RuleFor(x => x.DateViewed)
            .NotEmpty().WithMessage("Date viewed is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Date viewed cannot be in the future");
    }
}