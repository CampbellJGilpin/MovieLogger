using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using movielogger.dal.entities;
using movielogger.messaging.Configuration;
using movielogger.messaging.Models;
using movielogger.services.interfaces;

namespace movielogger.messaging.Services
{
    public class AuditEventConsumer : IDisposable
    {
        private readonly RabbitMQConsumer _consumer;
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditEventConsumer> _logger;

        public AuditEventConsumer(
            IOptions<RabbitMQConfig> config,
            IAuditService auditService,
            ILogger<AuditEventConsumer> logger,
            ILogger<RabbitMQConsumer> rabbitMQLogger)
        {
            _auditService = auditService;
            _logger = logger;
            
            // Create the underlying RabbitMQ consumer
            _consumer = new RabbitMQConsumer(config, rabbitMQLogger);
            _consumer.MessageReceived += OnMessageReceived;
        }

        public void StartConsuming()
        {
            _consumer.StartConsuming();
            _logger.LogInformation("Audit event consumer started");
        }

        public void StopConsuming()
        {
            _consumer.StopConsuming();
            _logger.LogInformation("Audit event consumer stopped");
        }

        private async void OnMessageReceived(object? sender, MovieEvent e)
        {
            try
            {
                _logger.LogInformation("Processing audit event: {EventType} for user {UserId}", e.EventType, e.UserId);

                var eventType = MapEventType(e.EventType);
                var description = GetEventDescription(e);
                var entityType = movielogger.dal.entities.EntityType.Movie;
                var entityId = GetEntityId(e);
                var additionalData = GetAdditionalData(e);

                await _auditService.LogEventAsync(
                    eventType: eventType,
                    userId: e.UserId,
                    description: description,
                    entityType: entityType,
                    entityId: entityId,
                    additionalData: additionalData
                );

                _logger.LogInformation("Successfully logged audit event: {EventType}", e.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing audit event: {EventType}", e.EventType);
            }
        }

        private EventType MapEventType(string eventTypeString)
        {
            return eventTypeString switch
            {
                "MovieAdded" => EventType.MovieAdded,
                "MovieUpdated" => EventType.MovieUpdated,
                "MovieDeleted" => EventType.MovieDeleted,
                "MovieFavorited" => EventType.MovieFavorited,
                "MovieAddedToLibrary" => EventType.MovieAddedToLibrary,
                "ReviewAdded" => EventType.ReviewAdded,
                "ReviewUpdated" => EventType.ReviewUpdated,
                "ReviewDeleted" => EventType.ReviewDeleted,
                "UserRegistered" => EventType.UserRegistered,
                "UserLoggedIn" => EventType.UserLoggedIn,
                "UserProfileUpdated" => EventType.UserProfileUpdated,
                _ => throw new ArgumentException($"Unknown event type: {eventTypeString}")
            };
        }

        private string GetEventDescription(MovieEvent e)
        {
            return e switch
            {
                MovieAddedEvent added => $"Movie '{added.MovieTitle}' was added to the system",
                MovieUpdatedEvent updated => $"Movie '{updated.MovieTitle}' was updated. Changed fields: {string.Join(", ", updated.ChangedFields)}",
                MovieDeletedEvent deleted => $"Movie '{deleted.MovieTitle}' was deleted from the system",
                MovieFavoritedEvent favorited => $"Movie '{favorited.MovieTitle}' was {(favorited.IsFavorited ? "marked as favorite" : "unmarked as favorite")}",
                MovieAddedToLibraryEvent library => $"Movie '{library.MovieTitle}' was {(library.AddedToLibrary ? "added to" : "removed from")} user's library",
                _ => $"Unknown movie event: {e.EventType}"
            };
        }

        private int? GetEntityId(MovieEvent e)
        {
            return e switch
            {
                MovieAddedEvent added => added.MovieId,
                MovieUpdatedEvent updated => updated.MovieId,
                MovieDeletedEvent deleted => deleted.MovieId,
                MovieFavoritedEvent favorited => favorited.MovieId,
                MovieAddedToLibraryEvent library => library.MovieId,
                _ => null
            };
        }

        private object? GetAdditionalData(MovieEvent e)
        {
            return e switch
            {
                MovieAddedEvent added => new { added.MovieTitle, added.Genre },
                MovieUpdatedEvent updated => new { updated.MovieTitle, updated.Genre, updated.ChangedFields },
                MovieDeletedEvent deleted => new { deleted.MovieTitle },
                MovieFavoritedEvent favorited => new { favorited.MovieTitle, favorited.IsFavorited },
                MovieAddedToLibraryEvent library => new { library.MovieTitle, library.AddedToLibrary },
                _ => null
            };
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
} 