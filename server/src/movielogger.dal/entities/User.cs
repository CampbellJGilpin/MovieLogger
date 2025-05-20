namespace movielogger.dal.entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool IsAdmin { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

    public ICollection<UserMovie> Library { get; set; } = new List<UserMovie>();
}
