using System.Text.Json.Serialization;

namespace movielogger.messaging.Models
{
    public abstract class MovieEvent
    {
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string RoutingKey => $"{EventType.ToLower()}.{UserId}";
    }

    public class MovieAddedEvent : MovieEvent
    {
        public MovieAddedEvent()
        {
            EventType = "MovieAdded";
        }
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
    }

    public class MovieUpdatedEvent : MovieEvent
    {
        public MovieUpdatedEvent()
        {
            EventType = "MovieUpdated";
        }
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string[] ChangedFields { get; set; } = Array.Empty<string>();
    }

    public class MovieDeletedEvent : MovieEvent
    {
        public MovieDeletedEvent()
        {
            EventType = "MovieDeleted";
        }
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
    }

    public class MovieFavoritedEvent : MovieEvent
    {
        public MovieFavoritedEvent()
        {
            EventType = "MovieFavorited";
        }
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public bool IsFavorited { get; set; }
    }

    public class MovieAddedToLibraryEvent : MovieEvent
    {
        public MovieAddedToLibraryEvent()
        {
            EventType = "MovieAddedToLibrary";
        }
        
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public bool AddedToLibrary { get; set; }
    }
} 