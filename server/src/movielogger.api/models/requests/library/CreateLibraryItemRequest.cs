using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;

namespace movielogger.api.models.requests.library;

public class CreateLibraryItemRequest : IValidatable<CreateLibraryItemRequest>
{
    public int MovieId { get; set; }
    public bool Favourite { get; set; }
    public bool OwnsMovie  { get; set; }
    public DateTime? UpcomingViewDate { get; set; }

    public IValidator<CreateLibraryItemRequest> GetValidator()
        => new CreateLibraryItemRequestValidator();
}