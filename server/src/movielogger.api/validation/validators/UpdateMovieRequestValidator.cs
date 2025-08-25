using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.movies;

namespace movielogger.api.validation.validators;

public class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.ReleaseDate).NotEmpty().WithMessage("ReleaseDate is required");
        RuleFor(x => x.GenreId).GreaterThan(0).WithMessage("GenreId must be greater than 0").When(x => x.GenreId.HasValue);
        RuleFor(x => x.RuntimeMinutes)
            .GreaterThan(0).WithMessage("Runtime must be greater than 0 minutes")
            .LessThanOrEqualTo(1000).WithMessage("Runtime must be less than 1000 minutes")
            .When(x => x.RuntimeMinutes.HasValue);
    }
}