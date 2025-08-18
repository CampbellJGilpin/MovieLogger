using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.lists;

public class CreateListRequest : IValidatable<CreateListRequest>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public IValidator<CreateListRequest> GetValidator()
        => new CreateListRequestValidator();
}