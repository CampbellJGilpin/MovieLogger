using movielogger.api.tests.fixtures;

namespace movielogger.api.tests.controllers;

public class BaseTestController : IDisposable
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _client;
    
    public BaseTestController()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }
    
    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
    }
}