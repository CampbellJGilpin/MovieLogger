using movielogger.services.interfaces;
using movielogger.services.services;

namespace movielogger.services.tests.services;

public class LibraryServiceTests : BaseServiceTest
{
    ILibraryService _service;
    
    public LibraryServiceTests()
    {
        _service = new LibraryService(_dbContext, _mapper);
    }
}