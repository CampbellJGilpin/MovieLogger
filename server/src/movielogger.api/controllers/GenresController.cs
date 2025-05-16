using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.models.requests.genres;
using movielogger.api.validators;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [Route("Genres")]
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
            var validator = new CreateGenreRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<CreateGenreRequest>(request);
            
            return Ok(mappedRequest);
        }

        [HttpPut("{genreId}")]
        public async Task<IActionResult> UpdateGenre(int genreId, [FromBody] UpdateGenreRequest request)
        {
            var validator = new UpdateGenreRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<UpdateGenreRequest>(request);
            
            return Ok(mappedRequest);
        }
    }
}
