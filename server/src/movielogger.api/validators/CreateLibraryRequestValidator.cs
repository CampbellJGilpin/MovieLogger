using FluentValidation;
using movielogger.api.models.requests.library;

namespace movielogger.api.validators;

public class CreateLibraryRequestValidator : AbstractValidator<CreateLibraryRequest>
{
    public CreateLibraryRequestValidator()
    {
            
    }
}