using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.genres;
using movielogger.api.models.responses.genres;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    //[Authorize]
    [Route("genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;

        public GenresController(IGenresService genresService, IMapper mapper)
        {
            _genresService = genresService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var serviceResponse = await _genresService.GetGenresAsync();
            var mappedResponse = _mapper.Map<List<GenreDto>>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenreById(int genreId)
        {
            var serviceResponse = await _genresService.GetGenreByIdAsync(genreId);
            var mappedResponse = _mapper.Map<GenreDto>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var mappedRequest = _mapper.Map<GenreDto>(request);
            var serviceResponse = await _genresService.CreateGenreAsync(mappedRequest);
            var mappedResponse = _mapper.Map<GenreResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPut("{genreId}")]
        public async Task<IActionResult> UpdateGenre(int genreId, [FromBody] UpdateGenreRequest request)
        {
            var errorResult = request.Validate();
            if (errorResult is not null) return errorResult;
            
            var mappedRequest = _mapper.Map<GenreDto>(request);
            var serviceResponse = await _genresService.UpdateGenreAsync(genreId, mappedRequest);
            var mappedResponse = _mapper.Map<GenreResponse>(serviceResponse);
            
            return Ok(mappedResponse);
        }
    }
}
