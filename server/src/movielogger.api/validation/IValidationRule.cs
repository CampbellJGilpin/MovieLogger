using Microsoft.AspNetCore.Mvc;

namespace movielogger.api.validation;

public interface IValidationRule<T>
{
    Task<ValidationResult> ValidateAsync(T model);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FieldName { get; set; }
}