using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.reviews;

public class UpdateReviewRequest : IValidatable<UpdateReviewRequest>
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }

    public IValidator<UpdateReviewRequest> GetValidator()
        => new UpdateReviewRequestValidator();
}