using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using Microsoft.AspNetCore.Http;

namespace movielogger.api.models.requests.movies;

public class CreateMovieRequest : IValidatable<CreateMovieRequest>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; } = DateTime.Today;
    public int GenreId { get; set; }
    public int? RuntimeMinutes { get; set; }
    public IFormFile? Poster { get; set; }

    public IValidator<CreateMovieRequest> GetValidator()
        => new CreateMovieRequestValidator();
}