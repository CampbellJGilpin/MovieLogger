using movielogger.services.interfaces;
using movielogger.services.services;

namespace movielogger.services.tests.services;

public class ViewingsServiceTests : BaseServiceTest
{
    IViewingsService _service;
    
    public ViewingsServiceTests()
    {
        _service = new ViewingsService(_dbContext, _mapper);
    }
}