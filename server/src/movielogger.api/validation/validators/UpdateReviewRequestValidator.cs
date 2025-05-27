using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.reviews;

namespace movielogger.api.validation.validators;

public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator()
    {
        RuleFor(x => x.ReviewText).NotEmpty().WithMessage("Review text cannot be empty");
        RuleFor(x => x.Score).GreaterThan(0).WithMessage("Score must be greater than 0");
    }
}