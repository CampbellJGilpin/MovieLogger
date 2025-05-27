using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.users;

public class UpdateUserRequest : IValidatable<UpdateUserRequest>
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsDeleted { get; set; }
    public IValidator<UpdateUserRequest> GetValidator()
        => new UpdateUserRequestValidator();
}