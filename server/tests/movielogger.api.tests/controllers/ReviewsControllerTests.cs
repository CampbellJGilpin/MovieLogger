using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace movielogger.api.tests.controllers;

public class ReviewsControllerTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ReviewsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}