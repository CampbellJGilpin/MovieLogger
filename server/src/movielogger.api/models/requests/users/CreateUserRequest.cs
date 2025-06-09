using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using System.Text.Json.Serialization;

namespace movielogger.api.models.requests.users;

public class CreateUserRequest : IValidatable<CreateUserRequest>
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    
    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; }
    
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
    
    public IValidator<CreateUserRequest> GetValidator()
        => new CreateUserRequestValidator();
}