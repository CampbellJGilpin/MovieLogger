using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.reviews;

public class CreateReviewRequest : IValidatable<CreateReviewRequest>
{
    public string ReviewText { get; set; } = string.Empty;
    public int Score { get; set; }
    public int UserId { get; set; }
    public DateTime? DateViewed { get; set; }

    public IValidator<CreateReviewRequest> GetValidator()
        => new CreateReviewRequestValidator();
}