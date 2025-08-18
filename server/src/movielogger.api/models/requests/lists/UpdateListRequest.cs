using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.lists;

public class UpdateListRequest : IValidatable<UpdateListRequest>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public IValidator<UpdateListRequest> GetValidator()
        => new UpdateListRequestValidator();
}