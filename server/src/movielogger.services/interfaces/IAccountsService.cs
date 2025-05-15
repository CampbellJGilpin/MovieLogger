namespace movielogger.services.interfaces;

public interface IAccountsService
{
    Task<bool> AuthenticateUserAsync(string username, string password);
}