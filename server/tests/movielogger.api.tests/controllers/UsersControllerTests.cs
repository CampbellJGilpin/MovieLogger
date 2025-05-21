using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.users;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class UsersControllerTests  : BaseTestController
{
    [Fact]
    public async Task Get_Always_ReturnsAllMovies()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task GetById_IfExists_ReturnsAllMovies()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
}