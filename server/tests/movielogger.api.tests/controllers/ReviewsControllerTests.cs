using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.reviews;
using movielogger.dal.dtos;
using NSubstitute;

namespace movielogger.api.tests.controllers;

public class ReviewsControllerTests  : BaseTestController
{
    [Fact]
    public async Task GetAllReviews_ReturnsSuccess()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task Post_WithValidData_SavesReview()
    {
        // Arrange 
        
        // Act
        
        // Assert
    }
}