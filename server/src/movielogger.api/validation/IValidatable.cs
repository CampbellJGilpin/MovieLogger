using FluentValidation;

namespace movielogger.api.validation;

public interface IValidatable<T>
{
    IValidator<T> GetValidator();
}