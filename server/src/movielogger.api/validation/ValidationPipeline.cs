using Microsoft.AspNetCore.Mvc;

namespace movielogger.api.validation;

public class ValidationPipeline<T> : IValidationPipeline<T>
{
    private readonly IEnumerable<IValidationRule<T>> _validationRules;

    public ValidationPipeline(IEnumerable<IValidationRule<T>> validationRules)
    {
        _validationRules = validationRules;
    }

    public async Task<IActionResult?> ValidateAsync(T model)
    {
        var errors = new List<object>();

        foreach (var rule in _validationRules)
        {
            var result = await rule.ValidateAsync(model);
            if (!result.IsValid)
            {
                errors.Add(new { Field = result.FieldName, Error = result.ErrorMessage });
            }
        }

        if (errors.Any())
        {
            return new BadRequestObjectResult(new { error = "Validation failed", details = errors });
        }

        return null;
    }
}

public interface IValidationPipeline<T>
{
    Task<IActionResult?> ValidateAsync(T model);
} 