namespace movielogger.api.models.responses.users;

public class LoginResponse
{
    public UserResponse User { get; set; } = null!;
    public string Token { get; set; } = null!;
}