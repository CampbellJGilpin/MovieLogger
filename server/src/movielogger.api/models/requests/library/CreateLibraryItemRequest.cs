using FluentValidation;
using movielogger.api.validation;
using movielogger.api.validation.validators;
using System.Text.Json.Serialization;

namespace movielogger.api.models.requests.library;

public class CreateLibraryItemRequest : IValidatable<CreateLibraryItemRequest>
{
    public int MovieId { get; set; }

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }

    public bool OwnsMovie { get; set; }
    public DateTime? UpcomingViewDate { get; set; }

    public IValidator<CreateLibraryItemRequest> GetValidator()
        => new CreateLibraryItemRequestValidator();
}