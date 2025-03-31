using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KPISolution.Models.Entities.Base;
using KPISolution.Models.Entities.KPI;

namespace KPISolution.Models.Entities.Measurement
{
    /// <summary>
    /// Represents a notification or alert related to a KPI
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>
        /// Reference to the KPI this notification is about
        /// </summary>
        [Display(Name = "KPI ID")]
        public Guid? KpiId { get; set; }

        /// <summary>
        /// Type of KPI (KRI, RI, PI)
        /// </summary>
        [StringLength(10)]
        [Display(Name = "KPI Type")]
        public string? KpiType { get; set; }

        /// <summary>
        /// Title of the notification
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed message for the notification
        /// </summary>
        [Required]
        [StringLength(500)]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Type of notification (Alert, Warning, Info, Success)
        /// </summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Type")]
        public string NotificationType { get; set; } = "Info";

        /// <summary>
        /// Priority level of the notification (High, Medium, Low)
        /// </summary>
        [Required]
        [StringLength(10)]
        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Date and time when the notification was triggered
        /// </summary>
        [Required]
        [Display(Name = "Notification Date")]
        [DataType(DataType.DateTime)]
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The actual value that triggered this notification
        /// </summary>
        [Display(Name = "Trigger Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? TriggerValue { get; set; }

        /// <summary>
        /// The threshold value that was breached
        /// </summary>
        [Display(Name = "Threshold Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ThresholdValue { get; set; }

        /// <summary>
        /// List of user IDs who should receive this notification
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Recipients")]
        public string? Recipients { get; set; }

        /// <summary>
        /// Whether the notification has been read
        /// </summary>
        [Display(Name = "Is Read")]
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date and time when the notification was read
        /// </summary>
        [Display(Name = "Read Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// Whether the notification requires acknowledgment
        /// </summary>
        [Display(Name = "Requires Acknowledgment")]
        public bool RequiresAcknowledgment { get; set; } = false;

        /// <summary>
        /// Date and time when the notification was acknowledged
        /// </summary>
        [Display(Name = "Acknowledgment Date")]
        [DataType(DataType.DateTime)]
        public DateTime? AcknowledgmentDate { get; set; }

        /// <summary>
        /// User who acknowledged the notification
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Acknowledged By")]
        public string? AcknowledgedBy { get; set; }

        /// <summary>
        /// Navigation property to KRI if KpiType is KRI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual KRI? KRI { get; set; }

        /// <summary>
        /// Navigation property to RI if KpiType is RI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual RI? RI { get; set; }

        /// <summary>
        /// Navigation property to PI if KpiType is PI
        /// </summary>
        [ForeignKey("KpiId")]
        public virtual PI? PI { get; set; }
    }
}
