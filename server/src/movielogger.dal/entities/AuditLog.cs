using System.ComponentModel.DataAnnotations;

namespace movielogger.dal.entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public EventType EventType { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [MaxLength(1000)]
        public string? AdditionalData { get; set; } // JSON string for extra event data
        
        public EntityType? EntityType { get; set; } // e.g., Movie, Review, etc.
        
        public int? EntityId { get; set; } // ID of the affected entity
        
        // Navigation properties to reference tables
        public virtual EventTypeReference? EventTypeReference { get; set; }
        public virtual EntityTypeReference? EntityTypeReference { get; set; }
    }
} 