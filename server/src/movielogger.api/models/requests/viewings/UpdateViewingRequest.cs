using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.viewings;

public class UpdateViewingRequest : IValidatable<UpdateViewingRequest>
{
    public DateTime DateViewed { get; set; }

    public IValidator<UpdateViewingRequest> GetValidator()
        => new UpdateViewingRequestValidator();
}