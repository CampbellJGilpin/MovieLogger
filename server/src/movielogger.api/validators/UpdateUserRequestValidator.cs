using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.users;

namespace movielogger.api.validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
            
    }
}