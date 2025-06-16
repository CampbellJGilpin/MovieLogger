using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.users;

public class ChangePasswordRequest : IValidatable<ChangePasswordRequest>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    
    public IValidator<ChangePasswordRequest> GetValidator()
        => new ChangePasswordRequestValidator();
} 