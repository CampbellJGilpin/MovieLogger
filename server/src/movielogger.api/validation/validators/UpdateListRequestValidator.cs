using FluentValidation;
using movielogger.api.models.requests.lists;

namespace movielogger.api.validation.validators;

public class UpdateListRequestValidator : AbstractValidator<UpdateListRequest>
{
    public UpdateListRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("List name is required")
            .MaximumLength(200).WithMessage("List name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}