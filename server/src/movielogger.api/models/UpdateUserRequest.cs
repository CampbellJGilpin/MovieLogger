namespace movielogger.api.models;

public class UpdateUserRequest
{
    public string UserName {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public bool IsAdmin {get;set;}
    public bool IsDeleted {get;set;}
}