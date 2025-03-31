using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Entities.Base
{
    /// <summary>
    /// Base class for all entities in the system
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Date and time when the entity was created
        /// </summary>
        [Required]
        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created the entity
        /// </summary>
        [Display(Name = "Created By")]
        [StringLength(50)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated
        /// </summary>
        [Required]
        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who last updated the entity
        /// </summary>
        [Display(Name = "Updated By")]
        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Flag indicating whether the entity is active
        /// </summary>
        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
