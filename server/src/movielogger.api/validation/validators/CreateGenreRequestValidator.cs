using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.genres;

namespace movielogger.api.validation.validators;

public class CreateGenreRequestValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required"); 
    }
}