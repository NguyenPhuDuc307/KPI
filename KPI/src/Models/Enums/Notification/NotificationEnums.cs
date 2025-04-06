namespace KPISolution.Models.Enums.Notification
{
    /// <summary>
    /// Severity levels for alerts and notifications
    /// </summary>
    public enum AlertSeverity
    {
        /// <summary>
        /// Information severity level
        /// </summary>
        [Display(Name = "Information")]
        Information = 1,

        /// <summary>
        /// Success severity level
        /// </summary>
        [Display(Name = "Success")]
        Success = 2,

        /// <summary>
        /// Warning severity level
        /// </summary>
        [Display(Name = "Warning")]
        Warning = 3,

        /// <summary>
        /// Error severity level
        /// </summary>
        [Display(Name = "Error")]
        Error = 4,

        /// <summary>
        /// Critical severity level
        /// </summary>
        [Display(Name = "Critical")]
        Critical = 5
    }

    /// <summary>
    /// Types of notifications in the system
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// KPI threshold breach notification
        /// </summary>
        [Display(Name = "KPI Threshold Breach")]
        KpiThresholdBreach = 1,

        /// <summary>
        /// KPI update notification
        /// </summary>
        [Display(Name = "KPI Update")]
        KpiUpdate = 2,

        /// <summary>
        /// Task assignment notification
        /// </summary>
        [Display(Name = "Task Assignment")]
        TaskAssignment = 3,

        /// <summary>
        /// Task completion notification
        /// </summary>
        [Display(Name = "Task Completion")]
        TaskCompletion = 4,

        /// <summary>
        /// Review reminder notification
        /// </summary>
        [Display(Name = "Review Reminder")]
        ReviewReminder = 5,

        /// <summary>
        /// Data submission reminder notification
        /// </summary>
        [Display(Name = "Data Submission Reminder")]
        DataSubmissionReminder = 6,

        /// <summary>
        /// Performance alert notification
        /// </summary>
        [Display(Name = "Performance Alert")]
        PerformanceAlert = 7,

        /// <summary>
        /// Goal achievement notification
        /// </summary>
        [Display(Name = "Goal Achievement")]
        GoalAchievement = 8,

        /// <summary>
        /// System notification
        /// </summary>
        [Display(Name = "System Notification")]
        SystemNotification = 9,

        /// <summary>
        /// Report generation notification
        /// </summary>
        [Display(Name = "Report Generation")]
        ReportGeneration = 10,

        /// <summary>
        /// Measurement due date notification
        /// </summary>
        [Display(Name = "Measurement Due")]
        MeasurementDue = 11,

        /// <summary>
        /// Comment notification
        /// </summary>
        [Display(Name = "Comment")]
        Comment = 12,

        /// <summary>
        /// Mention notification
        /// </summary>
        [Display(Name = "Mention")]
        Mention = 13,

        /// <summary>
        /// Status change notification
        /// </summary>
        [Display(Name = "Status Change")]
        StatusChange = 14,

        /// <summary>
        /// Risk alert notification
        /// </summary>
        [Display(Name = "Risk Alert")]
        RiskAlert = 15,

        /// <summary>
        /// Custom notification
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 16
    }

    /// <summary>
    /// Notification delivery channels
    /// </summary>
    public enum NotificationChannel
    {
        /// <summary>
        /// In-app notification
        /// </summary>
        [Display(Name = "In-App")]
        InApp = 1,

        /// <summary>
        /// Email notification
        /// </summary>
        [Display(Name = "Email")]
        Email = 2,

        /// <summary>
        /// SMS notification
        /// </summary>
        [Display(Name = "SMS")]
        SMS = 3,

        /// <summary>
        /// Push notification
        /// </summary>
        [Display(Name = "Push Notification")]
        PushNotification = 4,

        /// <summary>
        /// Slack notification
        /// </summary>
        [Display(Name = "Slack")]
        Slack = 5,

        /// <summary>
        /// Microsoft Teams notification
        /// </summary>
        [Display(Name = "Microsoft Teams")]
        MicrosoftTeams = 6,

        /// <summary>
        /// Webhook notification
        /// </summary>
        [Display(Name = "Webhook")]
        Webhook = 7,

        /// <summary>
        /// API notification
        /// </summary>
        [Display(Name = "API")]
        API = 8,

        /// <summary>
        /// Custom notification channel
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 9
    }

    /// <summary>
    /// Represents the priority of notifications
    /// </summary>
    public enum NotificationPriority
    {
        [Display(Name = "Low")]
        Low = 0,

        [Display(Name = "Medium")]
        Medium = 1,

        [Display(Name = "High")]
        High = 2,

        [Display(Name = "Critical")]
        Critical = 3
    }
}
