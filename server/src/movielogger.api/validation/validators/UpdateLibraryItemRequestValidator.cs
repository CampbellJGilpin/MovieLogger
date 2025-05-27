using FluentValidation;
using movielogger.api.models.requests.library;

namespace movielogger.api.validation.validators;

public class UpdateLibraryItemRequestValidator : AbstractValidator<UpdateLibraryItemRequest>
{
    public UpdateLibraryItemRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage("MovieId is required")
            .GreaterThan(0).WithMessage("MovieId must be greater than 0");
    }
}