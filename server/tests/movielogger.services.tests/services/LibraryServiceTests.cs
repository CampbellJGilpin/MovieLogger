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

    public async Task GetLibraryByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        
        // Act
        
        // Assert
    }

    public async Task GetLibraryFavouritesByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        
        // Act
        
        // Assert
    }

    public async Task GetLibraryWatchlistByUserIdAsync_ValidId_ReturnsMappedLibrary()
    {
        // Arrange
        
        // Act
        
        // Assert
    }

    public async Task CreateLibraryEntryAsync_ValidInput_AddsLibraryEntryAndReturnsDto()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
    
    public async Task UpdateLibraryEntryAsync_ExistingLibraryItem_AddsLibraryEntryAndReturnsDto()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
}