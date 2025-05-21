using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class GenresControllerTests : BaseTestController
{
    [Fact]
    public async Task Get_Always_ReturnsAllGenres()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task Get_IfExists_ReturnsGenre()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesGenre()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
}