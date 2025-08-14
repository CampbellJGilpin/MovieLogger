using movielogger.dal.entities;

namespace movielogger.services.interfaces
{
    public interface IAuditService
    {
        Task LogEventAsync(EventType eventType, string userId, string description, EntityType? entityType = null, int? entityId = null, object? additionalData = null);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page = 1, int pageSize = 10, EventType? eventType = null, EntityType? entityType = null, string? userId = null);
        Task<IEnumerable<EventTypeReference>> GetEventTypesAsync();
        Task<IEnumerable<EntityTypeReference>> GetEntityTypesAsync();
    }
}