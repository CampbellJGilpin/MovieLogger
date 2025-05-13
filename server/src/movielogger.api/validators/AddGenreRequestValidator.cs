using FluentValidation;
using movielogger.api.models;

namespace movielogger.api.validators;

public class AddGenreRequestValidator : AbstractValidator<AddGenreRequest>
{
    public AddGenreRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required"); 
    }
}