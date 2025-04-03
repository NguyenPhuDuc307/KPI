using System;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Entities.Base
{
    /// <summary>
    /// Base class for all entities in the system
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Date and time when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who created the entity
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who last updated the entity
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Whether the entity is deleted (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Whether the entity is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
