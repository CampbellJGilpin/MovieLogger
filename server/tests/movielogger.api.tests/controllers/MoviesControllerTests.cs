using System.Net.Http.Json;
using movielogger.api.models.requests.movies;
using movielogger.api.models.responses.movies;
using movielogger.api.tests.fixtures;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class MoviesControllerTests : BaseTestController
{
    [Fact]
    public async Task Get_Always_ReturnsAllMovies()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }

    [Fact]
    public async Task GetById_IfExists_ReturnsMovie()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesMovie()
    {
        // Arrange 
        
        // Act
        
        // Assert

    }
}