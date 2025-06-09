using movielogger.dal.entities;

namespace movielogger.services.interfaces;

public interface IAccountsService
{
    Task<(string token, User user)> AuthenticateUserAsync(string email, string password);
}