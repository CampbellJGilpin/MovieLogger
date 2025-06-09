using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace movielogger.api.validation;

public interface IValidatable<T>
{
    IValidator<T> GetValidator();
}

public static class ValidatableExtensions
{
    public static IActionResult? Validate<T>(this T model) where T : IValidatable<T>
    {
        var validator = model.GetValidator();
        var validationResult = validator.Validate(model);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => new { Field = x.PropertyName, Error = x.ErrorMessage });
            return new BadRequestObjectResult(new { error = "Validation failed", details = errors });
        }

        return null;
    }
}