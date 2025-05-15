using movielogger.services.interfaces;

namespace movielogger.services.services;

public class AccountsService : IAccountsService
{
    public Task<bool> AuthenticateUserAsync(string username, string password)
    {
        throw new NotImplementedException();
    }
}