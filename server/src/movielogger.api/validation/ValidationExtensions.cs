using Microsoft.AspNetCore.Mvc;

namespace movielogger.api.validation;

public static class ValidationExtensions
{
    public static IActionResult? Validate<T>(this IValidatable<T> request)
    {
        var result = request.GetValidator().Validate((T)request);

        if (!result.IsValid)
        {
            var errors = result
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return new BadRequestObjectResult(new
            {
                message = "Validation failed",
                errors
            });
        }

        return null;
    }
}