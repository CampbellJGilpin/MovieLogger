using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.movies;

namespace movielogger.api.validators;

public class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.ReleaseDate).NotEmpty().WithMessage("ReleaseDate is required");
        RuleFor(x => x.GenreId).GreaterThan(0).WithMessage("Genre must be specified.");
    }
}