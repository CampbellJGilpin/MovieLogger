using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.viewings;

namespace movielogger.api.validators;

public class UpdateViewingRequestValidator : AbstractValidator<UpdateViewingRequest>
{
    public UpdateViewingRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0")
            .NotEmpty().WithMessage("MovieId is required");
        RuleFor(x => x.DateViewed).NotEmpty().WithMessage("DateViewed is required.");
    }
}