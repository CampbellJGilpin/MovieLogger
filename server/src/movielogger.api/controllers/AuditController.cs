using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movielogger.dal.entities;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] EventType? eventType = null,
            [FromQuery] EntityType? entityType = null,
            [FromQuery] string? userId = null)
        {
            if (page < 1)
            {
                return BadRequest(new { error = "Page must be greater than 0" });
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { error = "Page size must be between 1 and 100" });
            }

            try
            {
                var logs = await _auditService.GetAuditLogsAsync(page, pageSize, eventType, entityType, userId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("event-types")]
        public async Task<ActionResult<IEnumerable<EventTypeReference>>> GetEventTypes()
        {
            try
            {
                var eventTypes = await _auditService.GetEventTypesAsync();
                return Ok(eventTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("entity-types")]
        public async Task<ActionResult<IEnumerable<EntityTypeReference>>> GetEntityTypes()
        {
            try
            {
                var entityTypes = await _auditService.GetEntityTypesAsync();
                return Ok(entityTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}