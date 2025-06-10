using movielogger.dal.entities;

namespace movielogger.services.interfaces;

public interface IAccountsService
{
    Task<(User user, string token)> AuthenticateUserAsync(string email, string password);
    Task<User> Register(string email, string password, string userName);
}