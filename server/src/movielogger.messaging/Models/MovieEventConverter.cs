using System.Text.Json;
using System.Text.Json.Serialization;

namespace movielogger.messaging.Models
{
    public class MovieEventConverter : JsonConverter<MovieEvent>
    {
        public override MovieEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected start of object");
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDocument.RootElement;

            if (!rootElement.TryGetProperty("eventType", out var eventTypeElement))
            {
                throw new JsonException("Missing 'eventType' property");
            }

            var eventType = eventTypeElement.GetString();
            if (string.IsNullOrEmpty(eventType))
            {
                throw new JsonException("'eventType' property is null or empty");
            }

            // Deserialize to the appropriate concrete type based on EventType
            return eventType switch
            {
                "MovieAdded" => JsonSerializer.Deserialize<MovieAddedEvent>(rootElement.GetRawText(), options),
                "MovieUpdated" => JsonSerializer.Deserialize<MovieUpdatedEvent>(rootElement.GetRawText(), options),
                "MovieDeleted" => JsonSerializer.Deserialize<MovieDeletedEvent>(rootElement.GetRawText(), options),
                "MovieFavorited" => JsonSerializer.Deserialize<MovieFavoritedEvent>(rootElement.GetRawText(), options),
                "MovieAddedToLibrary" => JsonSerializer.Deserialize<MovieAddedToLibraryEvent>(rootElement.GetRawText(), options),
                _ => throw new JsonException($"Unknown event type: {eventType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, MovieEvent value, JsonSerializerOptions options)
        {
            // For serialization, we can use the default behavior since we're writing concrete types
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
} 