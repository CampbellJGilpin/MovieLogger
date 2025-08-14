using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.viewings;

namespace movielogger.api.validation.validators;

public class CreateViewingRequestValidator : AbstractValidator<CreateViewingRequest>
{
    public CreateViewingRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0")
            .NotEmpty().WithMessage("MovieId is required");

        RuleFor(x => x.DateViewed)
            .NotEmpty().WithMessage("DateViewed is required")
            .Must(date => date <= DateTime.Now).WithMessage("DateViewed cannot be in the future");
    }
}