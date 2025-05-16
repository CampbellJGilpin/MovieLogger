namespace movielogger.api.models.responses.users;

public class UserResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsDeleted { get; set; }
}