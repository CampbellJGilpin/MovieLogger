using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.reviews;

namespace movielogger.api.validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.ReviewText).NotEmpty().WithMessage("Review text cannot be empty");
        RuleFor(x => x.Score).GreaterThan(0).LessThanOrEqualTo(5).WithMessage("Score must be between 1 and 5");
    }
}