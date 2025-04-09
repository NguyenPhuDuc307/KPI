using System.ComponentModel.DataAnnotations.Schema;

namespace KPISolution.Models.Entities.Notification
{
    /// <summary>
    /// Represents a notification in the system
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>
        /// Title of the notification
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Message content of the notification
        /// </summary>
        [Required]
        [StringLength(1000)]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Type of notification
        /// </summary>
        [Required]
        [Display(Name = "Type")]
        public NotificationType Type { get; set; } = NotificationType.Custom;

        /// <summary>
        /// Priority level of the notification
        /// </summary>
        [Required]
        [Display(Name = "Priority")]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        /// <summary>
        /// Gets or sets the date and time when the notification was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the notification expires
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Expires At")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Target user ID who should receive this notification
        /// </summary>
        [StringLength(450)]
        [Display(Name = "User ID")]
        public string? UserId { get; set; }

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Whether the notification has been read by the user
        /// </summary>
        [Display(Name = "Is Read")]
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date and time when the notification was read
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Read At")]
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// Whether the notification has been sent via email
        /// </summary>
        [Display(Name = "Is Sent")]
        public bool IsSent { get; set; } = false;

        /// <summary>
        /// Date and time when the notification was sent
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Sent At")]
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// Whether the notification is related to a KPI
        /// </summary>
        [Display(Name = "Is KPI Notification")]
        public bool IsKpiNotification { get; set; } = false;

        /// <summary>
        /// KPI ID if this notification is related to a KPI
        /// </summary>
        [Display(Name = "KPI ID")]
        public Guid? KpiId { get; set; }

        /// <summary>
        /// Custom data as JSON string
        /// </summary>
        [Display(Name = "Data")]
        public string? Data { get; set; }

        /// <summary>
        /// Reference to an entity related to this notification
        /// </summary>
        [Display(Name = "Reference ID")]
        public Guid? ReferenceId { get; set; }

        /// <summary>
        /// Type of the referenced entity
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Reference Type")]
        public string? ReferenceType { get; set; }
    }
}
