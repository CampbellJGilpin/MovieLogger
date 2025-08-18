using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models.requests.lists;
using movielogger.api.models.responses.lists;
using movielogger.api.models.responses.movies;
using movielogger.api.validation;
using movielogger.services.interfaces;
using System.Security.Claims;

namespace movielogger.api.controllers;

[Authorize]
[ApiController]
[Route("api/users/{userId}/lists")]
public class ListsController : ControllerBase
{
    private readonly IListsService _listsService;
    private readonly IMapper _mapper;

    public ListsController(IListsService listsService, IMapper mapper)
    {
        _listsService = listsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserLists(int userId)
    {
        var result = await _listsService.GetUserListsAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var response = _mapper.Map<IEnumerable<ListSummaryResponse>>(result.Value);
        return Ok(response);
    }

    [HttpGet("{listId}")]
    public async Task<IActionResult> GetList(int userId, int listId)
    {
        var result = await _listsService.GetListByIdAsync(listId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        if (result.Value == null)
        {
            return NotFound($"List with ID {listId} not found");
        }

        var response = _mapper.Map<ListResponse>(result.Value);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateList(int userId, [FromBody] CreateListRequest request)
    {
        var errorResult = request.Validate();
        if (errorResult is not null)
            return BadRequest(((BadRequestObjectResult)errorResult).Value);

        var result = await _listsService.CreateListAsync(userId, request.Name, request.Description);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var response = _mapper.Map<ListSummaryResponse>(result.Value);
        return CreatedAtAction(nameof(GetList), new { userId = userId, listId = response.Id }, response);
    }

    [HttpPut("{listId}")]
    public async Task<IActionResult> UpdateList(int userId, int listId, [FromBody] UpdateListRequest request)
    {
        var errorResult = request.Validate();
        if (errorResult is not null)
            return BadRequest(((BadRequestObjectResult)errorResult).Value);

        var result = await _listsService.UpdateListAsync(listId, userId, request.Name, request.Description);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var response = _mapper.Map<ListSummaryResponse>(result.Value);
        return Ok(response);
    }

    [HttpDelete("{listId}")]
    public async Task<IActionResult> DeleteList(int userId, int listId)
    {
        var result = await _listsService.DeleteListAsync(listId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpPost("{listId}/movies")]
    public async Task<IActionResult> AddMovieToList(int userId, int listId, [FromBody] AddMovieToListRequest request)
    {
        var errorResult = request.Validate();
        if (errorResult is not null)
            return BadRequest(((BadRequestObjectResult)errorResult).Value);

        var result = await _listsService.AddMovieToListAsync(listId, request.MovieId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { message = "Movie added to list successfully" });
    }

    [HttpDelete("{listId}/movies/{movieId}")]
    public async Task<IActionResult> RemoveMovieFromList(int userId, int listId, int movieId)
    {
        var result = await _listsService.RemoveMovieFromListAsync(listId, movieId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("{listId}/movies")]
    public async Task<IActionResult> GetListMovies(int userId, int listId)
    {
        var result = await _listsService.GetListMoviesAsync(listId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var response = _mapper.Map<IEnumerable<MovieResponse>>(result.Value);
        return Ok(response);
    }

    [HttpGet("{listId}/movies/{movieId}/check")]
    public async Task<IActionResult> CheckMovieInList(int userId, int listId, int movieId)
    {
        var result = await _listsService.IsMovieInListAsync(listId, movieId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { inList = result.Value });
    }
}