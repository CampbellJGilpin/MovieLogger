using movielogger.api.validation;
using movielogger.api.validation.validators;
using FluentValidation;

namespace movielogger.api.models.requests.users;

public class RegisterRequest : IValidatable<RegisterRequest>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public IValidator<RegisterRequest> GetValidator()
    {
        return new RegisterRequestValidator();
    }
}