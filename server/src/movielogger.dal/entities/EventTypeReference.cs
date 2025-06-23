using System.ComponentModel.DataAnnotations;

namespace movielogger.dal.entities
{
    public class EventTypeReference
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public EventType EventType { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property to AuditLog
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }

    public class EntityTypeReference
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public EntityType EntityType { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property to AuditLog
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
} 