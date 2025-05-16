namespace movielogger.api.models.requests.users;

public class UpdateUserRequest
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsDeleted { get; set; }
}