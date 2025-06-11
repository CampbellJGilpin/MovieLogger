using movielogger.api.tests.fixtures;
using movielogger.api.tests.helpers;
using movielogger.dal.entities;

namespace movielogger.api.tests.controllers;

public class BaseTestController : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _client;
    protected User _testUser;
    
    public BaseTestController()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        
        // Set up test user and authentication
        _testUser = TestDataBuilder.CreateTestUser(1, "John Doe", "johndoe@example.com");
        var token = AuthenticationHelper.GenerateJwtToken(_testUser);
        AuthenticationHelper.AddAuthorizationHeader(_client, token);
    }

    public Task InitializeAsync()
    {
        _factory.ResetDatabase();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }
}