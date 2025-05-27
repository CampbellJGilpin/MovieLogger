using FluentValidation;
using movielogger.api.models.requests.library;

namespace movielogger.api.validation.validators;

public class CreateLibraryItemRequestValidator : AbstractValidator<CreateLibraryItemRequest>
{
    public CreateLibraryItemRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage("MovieId is required")
            .GreaterThan(0).WithMessage("MovieId must be greater than 0");
    }
}