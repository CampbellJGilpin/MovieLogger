using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;
using movielogger.dal;
using movielogger.services.interfaces;
using System.Text.Json;

namespace movielogger.services.services
{
    public class AuditService : IAuditService
    {
        private readonly IAssessmentDbContext _context;

        public AuditService(IAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task LogEventAsync(EventType eventType, string userId, string description, EntityType? entityType = null, int? entityId = null, object? additionalData = null)
        {
            var auditLog = new AuditLog
            {
                EventType = eventType,
                UserId = userId,
                Description = description,
                Timestamp = DateTime.UtcNow,
                EntityType = entityType,
                EntityId = entityId,
                AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page = 1, int pageSize = 10, EventType? eventType = null, EntityType? entityType = null, string? userId = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.EventTypeReference)
                .Include(a => a.EntityTypeReference)
                .AsQueryable();

            if (eventType.HasValue)
                query = query.Where(a => a.EventType == eventType.Value);

            if (entityType.HasValue)
                query = query.Where(a => a.EntityType == entityType.Value);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a => a.UserId == userId);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventTypeReference>> GetEventTypesAsync()
        {
            return await _context.EventTypeReferences
                .Where(et => et.IsActive)
                .OrderBy(et => et.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<EntityTypeReference>> GetEntityTypesAsync()
        {
            return await _context.EntityTypeReferences
                .Where(et => et.IsActive)
                .OrderBy(et => et.Name)
                .ToListAsync();
        }
    }
} 