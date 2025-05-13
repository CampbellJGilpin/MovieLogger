using FluentValidation;
using movielogger.api.models;

namespace movielogger.api.validators;

public class CreateGenreRequestValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required"); 
    }
}