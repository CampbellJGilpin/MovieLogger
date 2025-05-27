using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.viewings;

public class CreateViewingRequest : IValidatable<CreateViewingRequest>
{
    public int MovieId { get; set; }
    public DateTime DateViewed { get; set; }

    public IValidator<CreateViewingRequest> GetValidator()
        => new CreateViewingRequestValidator();
}