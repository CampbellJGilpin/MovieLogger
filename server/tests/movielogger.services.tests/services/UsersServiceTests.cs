using movielogger.services.interfaces;
using movielogger.services.services;

namespace movielogger.services.tests.services;

public class UsersServiceTests : BaseServiceTest
{
    IUsersService _service;
    
    public UsersServiceTests()
    {
        _service = new UsersService(_dbContext, _mapper);
    }
}