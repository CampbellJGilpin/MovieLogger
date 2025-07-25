using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.responses.genres;
using movielogger.dal.dtos;
using movielogger.services.interfaces;
using System.Security.Claims;

namespace movielogger.api.controllers;

[Authorize]
[Route("api/genre-preferences")]
[ApiController]
public class GenrePreferencesController : ControllerBase
{
    private readonly IGenrePreferencesService _genrePreferencesService;
    private readonly IMapper _mapper;

    public GenrePreferencesController(IGenrePreferencesService genrePreferencesService, IMapper mapper)
    {
        _genrePreferencesService = genrePreferencesService;
        _mapper = mapper;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetUserGenrePreferences()
    {
        try
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var preferences = await _genrePreferencesService.GetUserGenrePreferencesAsync(userId);
            var response = _mapper.Map<GenrePreferencesSummaryResponse>(preferences);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving genre preferences");
        }
    }

    [HttpGet("top-by-watches")]
    public async Task<IActionResult> GetTopGenresByWatchCount([FromQuery] int count = 5)
    {
        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var genres = await _genrePreferencesService.GetTopGenresByWatchCountAsync(userId, count);
            var response = _mapper.Map<List<GenrePreferenceResponse>>(genres);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving top genres");
        }
    }

    [HttpGet("top-by-rating")]
    public async Task<IActionResult> GetTopGenresByRating([FromQuery] int count = 5)
    {
        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var genres = await _genrePreferencesService.GetTopGenresByRatingAsync(userId, count);
            var response = _mapper.Map<List<GenrePreferenceResponse>>(genres);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving top rated genres");
        }
    }

    [HttpGet("least-watched")]
    public async Task<IActionResult> GetLeastWatchedGenres([FromQuery] int count = 5)
    {
        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var genres = await _genrePreferencesService.GetLeastWatchedGenresAsync(userId, count);
            var response = _mapper.Map<List<GenrePreferenceResponse>>(genres);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving least watched genres");
        }
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetGenreWatchTrends([FromQuery] int months = 6)
    {
        if (months <= 0 || months > 60)
        {
            return BadRequest("Months must be between 1 and 60");
        }

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var trends = await _genrePreferencesService.GetGenreWatchTrendsAsync(userId, months);
            
            return Ok(trends);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving genre trends");
        }
    }
} 