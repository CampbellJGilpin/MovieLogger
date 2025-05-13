using FluentValidation;
using movielogger.api.models;

namespace movielogger.api.validators;

public class CreateViewingRequestValidator : AbstractValidator<CreateViewingRequest>
{
    public CreateViewingRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0")
            .NotEmpty().WithMessage("MovieId is required");
        RuleFor(x => x.DateViewed).NotEmpty().WithMessage("DateViewed is required");
    }
}