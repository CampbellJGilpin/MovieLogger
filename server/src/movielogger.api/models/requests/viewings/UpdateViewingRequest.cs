using FluentValidation;
using movielogger.api.models.requests.reviews;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.viewings;

public class UpdateViewingRequest : IValidatable<UpdateReviewRequest>
{
    public DateTime DateViewed { get; set; }

    public IValidator<UpdateReviewRequest> GetValidator()
        => new UpdateReviewRequestValidator();
}