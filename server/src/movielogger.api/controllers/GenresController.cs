using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.api.validation;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [Route("api/genres")]
    [ApiController]
    public class GenresController(IGenresService genresService, IMapper mapper) : ControllerBase
    {
        private readonly IGenresService _genresService = genresService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var serviceResponse = await _genresService.GetGenresAsync();
            var mappedResponse = _mapper.Map<List<GenreResponse>>(serviceResponse);

            return Ok(mappedResponse);
        }

        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenreById(int genreId)
        {
            try
            {
                var serviceResponse = await _genresService.GetGenreByIdAsync(genreId);
                if (serviceResponse == null)
                    return NotFound($"Genre with ID {genreId} not found.");
                var mappedResponse = _mapper.Map<GenreResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Genre with ID {genreId} not found.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var mappedRequest = _mapper.Map<GenreDto>(request);
                var serviceResponse = await _genresService.CreateGenreAsync(mappedRequest);
                if (serviceResponse == null)
                    return BadRequest("Failed to create genre");
                var mappedResponse = _mapper.Map<GenreResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{genreId}")]
        public async Task<IActionResult> UpdateGenre(int genreId, [FromBody] UpdateGenreRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return BadRequest(((BadRequestObjectResult)errorResult).Value);

            try
            {
                var mappedRequest = _mapper.Map<GenreDto>(request);
                var serviceResponse = await _genresService.UpdateGenreAsync(genreId, mappedRequest);
                if (serviceResponse == null)
                    return NotFound($"Genre with ID {genreId} not found.");
                var mappedResponse = _mapper.Map<GenreResponse>(serviceResponse);

                return Ok(mappedResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Genre with ID {genreId} not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{genreId}")]
        public async Task<IActionResult> DeleteGenre(int genreId)
        {
            try
            {
                await _genresService.DeleteGenreAsync(genreId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Genre with ID {genreId} not found.");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Cannot delete genre that has associated movies.");
            }
        }
    }
}
