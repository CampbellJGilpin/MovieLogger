using FluentValidation;
using movielogger.api.models.requests.library;

namespace movielogger.api.validators;

public class UpdateLibraryRequestValidator : AbstractValidator<UpdateLibraryRequest>
{
    public UpdateLibraryRequestValidator()
    {
            
    }
}