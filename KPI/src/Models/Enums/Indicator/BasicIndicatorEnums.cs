namespace KPISolution.Models.Enums.Indicator
{
    /// <summary>
    /// Enum representing basic indicator types in the system
    /// </summary>
    public enum BasicIndicatorType
    {
        /// <summary>
        /// Key Result Indicator
        /// </summary>
        [Display(Name = "Key Result Indicator (KRI)")]
        KRI = 1,

        /// <summary>
        /// Result Indicator
        /// </summary>
        [Display(Name = "Result Indicator (RI)")]
        RI = 2,

        /// <summary>
        /// Performance Indicator
        /// </summary>
        [Display(Name = "Performance Indicator (PI)")]
        PI = 3,

        /// <summary>
        /// Key Performance Indicator
        /// </summary>
        [Display(Name = "Key Performance Indicator (KPI)")]
        KPI = 4
    }

    /// <summary>
    /// Status options for Performance Indicators
    /// </summary>
    public enum IndicatorStatus
    {
        /// <summary>
        /// Draft status - not yet active
        /// </summary>
        [Display(Name = "Draft")]
        Draft = 1,

        /// <summary>
        /// Active status - currently being tracked
        /// </summary>
        [Display(Name = "Active")]
        Active = 2,

        /// <summary>
        /// Under review status
        /// </summary>
        [Display(Name = "Under Review")]
        UnderReview = 3,

        /// <summary>
        /// Archived status - no longer active
        /// </summary>
        [Display(Name = "Archived")]
        Archived = 4,

        /// <summary>
        /// Approved status
        /// </summary>
        [Display(Name = "Approved")]
        Approved = 5,

        /// <summary>
        /// On target status
        /// </summary>
        [Display(Name = "On Target")]
        OnTarget = 6,

        /// <summary>
        /// At risk status
        /// </summary>
        [Display(Name = "At Risk")]
        AtRisk = 7,

        /// <summary>
        /// Below target status
        /// </summary>
        [Display(Name = "Below Target")]
        BelowTarget = 8,

        /// <summary>
        /// Deprecated status
        /// </summary>
        [Display(Name = "Deprecated")]
        Deprecated = 9
    }

    /// <summary>
    /// Categories of Performance Indicators
    /// </summary>
    public enum IndicatorCategory
    {
        /// <summary>
        /// Financial category
        /// </summary>
        [Display(Name = "Financial")]
        Financial = 1,

        /// <summary>
        /// Customer category
        /// </summary>
        [Display(Name = "Customer")]
        Customer = 2,

        /// <summary>
        /// Internal business process category
        /// </summary>
        [Display(Name = "Internal Business")]
        InternalBusiness = 3,

        /// <summary>
        /// Learning and growth category
        /// </summary>
        [Display(Name = "Learning & Growth")]
        LearningAndGrowth = 4,

        /// <summary>
        /// Operations category
        /// </summary>
        [Display(Name = "Operations")]
        Operations = 5,

        /// <summary>
        /// Quality category
        /// </summary>
        [Display(Name = "Quality")]
        Quality = 6,

        /// <summary>
        /// Human resources category
        /// </summary>
        [Display(Name = "Human Resources")]
        HumanResources = 7,

        /// <summary>
        /// Information technology category
        /// </summary>
        [Display(Name = "IT")]
        IT = 8,

        /// <summary>
        /// Environmental category
        /// </summary>
        [Display(Name = "Environmental")]
        Environmental = 9,

        /// <summary>
        /// Social category
        /// </summary>
        [Display(Name = "Social")]
        Social = 10,

        /// <summary>
        /// Governance category
        /// </summary>
        [Display(Name = "Governance")]
        Governance = 11,

        /// <summary>
        /// Innovation category
        /// </summary>
        [Display(Name = "Innovation")]
        Innovation = 12,

        /// <summary>
        /// Productivity category
        /// </summary>
        [Display(Name = "Productivity")]
        Productivity = 13,

        /// <summary>
        /// Safety category
        /// </summary>
        [Display(Name = "Safety")]
        Safety = 14,

        /// <summary>
        /// Project category
        /// </summary>
        [Display(Name = "Project")]
        Project = 15,

        /// <summary>
        /// Risk category
        /// </summary>
        [Display(Name = "Risk")]
        Risk = 16,

        /// <summary>
        /// Other category
        /// </summary>
        [Display(Name = "Other")]
        Other = 99
    }

    /// <summary>
    /// Enum representing different impact levels of indicators
    /// </summary>
    public enum ImpactLevel
    {
        /// <summary>
        /// Low impact
        /// </summary>
        [Display(Name = "Low")]
        Low = 1,

        /// <summary>
        /// Medium impact
        /// </summary>
        [Display(Name = "Medium")]
        Medium = 2,

        /// <summary>
        /// High impact
        /// </summary>
        [Display(Name = "High")]
        High = 3,

        /// <summary>
        /// Critical impact
        /// </summary>
        [Display(Name = "Critical")]
        Critical = 4
    }

    /// <summary>
    /// Enum representing different business areas
    /// </summary>
    public enum BusinessArea
    {
        /// <summary>
        /// General business area
        /// </summary>
        [Display(Name = "General")]
        General = 1,

        /// <summary>
        /// Sales business area
        /// </summary>
        [Display(Name = "Sales")]
        Sales = 2,

        /// <summary>
        /// Marketing business area
        /// </summary>
        [Display(Name = "Marketing")]
        Marketing = 3,

        /// <summary>
        /// Finance business area
        /// </summary>
        [Display(Name = "Finance")]
        Finance = 4,

        /// <summary>
        /// Human Resources business area
        /// </summary>
        [Display(Name = "Human Resources")]
        HR = 5,

        /// <summary>
        /// Operations business area
        /// </summary>
        [Display(Name = "Operations")]
        Operations = 6,

        /// <summary>
        /// Information Technology business area
        /// </summary>
        [Display(Name = "IT")]
        IT = 7,

        /// <summary>
        /// Research and Development business area
        /// </summary>
        [Display(Name = "R&D")]
        RnD = 8,

        /// <summary>
        /// Customer Service business area
        /// </summary>
        [Display(Name = "Customer Service")]
        CustomerService = 9,

        /// <summary>
        /// Production business area
        /// </summary>
        [Display(Name = "Production")]
        Production = 10,

        /// <summary>
        /// Supply Chain business area
        /// </summary>
        [Display(Name = "Supply Chain")]
        SupplyChain = 11,

        /// <summary>
        /// Quality business area
        /// </summary>
        [Display(Name = "Quality")]
        Quality = 12,

        /// <summary>
        /// Other business area
        /// </summary>
        [Display(Name = "Other")]
        Other = 13
    }

    /// <summary>
    /// Enum representing different activity types
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Business Process activity type
        /// </summary>
        [Display(Name = "Business Process")]
        BusinessProcess = 1,

        /// <summary>
        /// Project activity type
        /// </summary>
        [Display(Name = "Project")]
        Project = 2,

        /// <summary>
        /// Operational activity type
        /// </summary>
        [Display(Name = "Operational")]
        Operational = 3,

        /// <summary>
        /// Strategic activity type
        /// </summary>
        [Display(Name = "Strategic")]
        Strategic = 4,

        /// <summary>
        /// Compliance activity type
        /// </summary>
        [Display(Name = "Compliance")]
        Compliance = 5,

        /// <summary>
        /// Innovation activity type
        /// </summary>
        [Display(Name = "Innovation")]
        Innovation = 6,

        /// <summary>
        /// Maintenance activity type
        /// </summary>
        [Display(Name = "Maintenance")]
        Maintenance = 7,

        /// <summary>
        /// Other activity type
        /// </summary>
        [Display(Name = "Other")]
        Other = 8
    }

    /// <summary>
    /// Enum representing indicator hierarchy types
    /// </summary>
    public enum IndicatorHierarchyType
    {
        /// <summary>
        /// Objective - Strategic goals or objectives
        /// </summary>
        [Display(Name = "Objective")]
        Objective = 1,

        /// <summary>
        /// Success Factor - Factors contributing to objectives
        /// </summary>
        [Display(Name = "Success Factor")]
        SuccessFactor = 2,

        /// <summary>
        /// Critical Success Factor - Critical elements for strategy success
        /// </summary>
        [Display(Name = "Critical Success Factor")]
        CriticalSuccessFactor = 3,

        /// <summary>
        /// Result Indicator - Measures what has been achieved
        /// </summary>
        [Display(Name = "Result Indicator")]
        ResultIndicator = 4,

        /// <summary>
        /// Performance Indicator - Measures what to do to improve performance
        /// </summary>
        [Display(Name = "Performance Indicator")]
        PerformanceIndicator = 5,

        /// <summary>
        /// Key Result Indicator - Key measures of what has been achieved
        /// </summary>
        [Display(Name = "Key Result Indicator")]
        KeyResultIndicator = 6,

        /// <summary>
        /// Key Performance Indicator - Key measures of what to do to improve performance
        /// </summary>
        [Display(Name = "Key Performance Indicator")]
        KeyPerformanceIndicator = 7
    }

    /// <summary>
    /// Enum representing different indicator timing types
    /// </summary>
    public enum IndicatorTimingType
    {
        /// <summary>
        /// Leading indicator - predictive measure
        /// </summary>
        [Display(Name = "Leading")]
        Leading = 1,

        /// <summary>
        /// Lagging indicator - output measure
        /// </summary>
        [Display(Name = "Lagging")]
        Lagging = 2,

        /// <summary>
        /// Coincident indicator - real-time measure
        /// </summary>
        [Display(Name = "Coincident")]
        Coincident = 3
    }

    /// <summary>
    /// Permission levels for Performance Indicator access
    /// </summary>
    public enum IndicatorAccessPermission
    {
        /// <summary>
        /// View permission
        /// </summary>
        [Display(Name = "View")]
        View = 1,

        /// <summary>
        /// Edit permission
        /// </summary>
        [Display(Name = "Edit")]
        Edit = 2,

        /// <summary>
        /// Delete permission
        /// </summary>
        [Display(Name = "Delete")]
        Delete = 3,

        /// <summary>
        /// Approve permission
        /// </summary>
        [Display(Name = "Approve")]
        Approve = 4,

        /// <summary>
        /// Measure permission
        /// </summary>
        [Display(Name = "Measure")]
        Measure = 5,

        /// <summary>
        /// Admin permission
        /// </summary>
        [Display(Name = "Admin")]
        Admin = 10
    }

    /// <summary>
    /// Permission level in the system
    /// </summary>
    public enum IndicatorPermissionLevel
    {
        /// <summary>
        /// None permission
        /// </summary>
        [Display(Name = "None")]
        None = 0,

        /// <summary>
        /// Read permission
        /// </summary>
        [Display(Name = "Read")]
        Read = 1,

        /// <summary>
        /// Write permission
        /// </summary>
        [Display(Name = "Write")]
        Write = 2,

        /// <summary>
        /// Delete permission
        /// </summary>
        [Display(Name = "Delete")]
        Delete = 3,

        /// <summary>
        /// Admin permission
        /// </summary>
        [Display(Name = "Admin")]
        Admin = 4
    }

    /// <summary>
    /// Represents the scope of measurement
    /// </summary>
    public enum MeasurementScope
    {
        /// <summary>
        /// Individual scope
        /// </summary>
        [Display(Name = "Individual")]
        Individual = 1,

        /// <summary>
        /// Team scope
        /// </summary>
        [Display(Name = "Team")]
        Team = 2,

        /// <summary>
        /// Department scope
        /// </summary>
        [Display(Name = "Department")]
        Department = 3,

        /// <summary>
        /// Division scope
        /// </summary>
        [Display(Name = "Division")]
        Division = 4,

        /// <summary>
        /// Organization scope
        /// </summary>
        [Display(Name = "Organization")]
        Organization = 5,

        /// <summary>
        /// Project scope
        /// </summary>
        [Display(Name = "Project")]
        Project = 6,

        /// <summary>
        /// Process scope
        /// </summary>
        [Display(Name = "Process")]
        Process = 7,

        /// <summary>
        /// Other scope
        /// </summary>
        [Display(Name = "Other")]
        Other = 8
    }

    /// <summary>
    /// Represents process areas in the organization
    /// </summary>
    public enum ProcessArea
    {
        /// <summary>
        /// Core Business Processes
        /// </summary>
        [Display(Name = "Core Business")]
        CoreBusiness = 1,

        /// <summary>
        /// Support Processes
        /// </summary>
        [Display(Name = "Support")]
        Support = 2,

        /// <summary>
        /// Management Processes
        /// </summary>
        [Display(Name = "Management")]
        Management = 3,

        /// <summary>
        /// Strategic Processes
        /// </summary>
        [Display(Name = "Strategic")]
        Strategic = 4,

        /// <summary>
        /// Operational Processes
        /// </summary>
        [Display(Name = "Operational")]
        Operational = 5,

        /// <summary>
        /// Innovation Processes
        /// </summary>
        [Display(Name = "Innovation")]
        Innovation = 6,

        /// <summary>
        /// Customer-facing Processes
        /// </summary>
        [Display(Name = "Customer-facing")]
        CustomerFacing = 7,

        /// <summary>
        /// Administrative Processes
        /// </summary>
        [Display(Name = "Administrative")]
        Administrative = 8,

        /// <summary>
        /// Compliance Processes
        /// </summary>
        [Display(Name = "Compliance")]
        Compliance = 9,

        /// <summary>
        /// Other Processes
        /// </summary>
        [Display(Name = "Other")]
        Other = 10
    }

    /// <summary>
    /// Represents time frames for indicators
    /// </summary>
    public enum TimeFrame
    {
        /// <summary>
        /// Daily time frame
        /// </summary>
        [Display(Name = "Daily")]
        Daily = 1,

        /// <summary>
        /// Weekly time frame
        /// </summary>
        [Display(Name = "Weekly")]
        Weekly = 2,

        /// <summary>
        /// Monthly time frame
        /// </summary>
        [Display(Name = "Monthly")]
        Monthly = 3,

        /// <summary>
        /// Quarterly time frame
        /// </summary>
        [Display(Name = "Quarterly")]
        Quarterly = 4,

        /// <summary>
        /// Annual time frame
        /// </summary>
        [Display(Name = "Annual")]
        Annual = 5,

        /// <summary>
        /// Multi-year time frame
        /// </summary>
        [Display(Name = "Multi-year")]
        MultiYear = 6,

        /// <summary>
        /// Project-based time frame
        /// </summary>
        [Display(Name = "Project-based")]
        ProjectBased = 7,

        /// <summary>
        /// Custom time frame
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 8
    }

    /// <summary>
    /// Represents types of results for result indicators
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Quantitative results that can be measured numerically
        /// </summary>
        [Display(Name = "Quantitative")]
        Quantitative = 1,

        /// <summary>
        /// Qualitative results that are descriptive
        /// </summary>
        [Display(Name = "Qualitative")]
        Qualitative = 2,

        /// <summary>
        /// Financial results related to money
        /// </summary>
        [Display(Name = "Financial")]
        Financial = 3,

        /// <summary>
        /// Operational results related to operations
        /// </summary>
        [Display(Name = "Operational")]
        Operational = 4,

        /// <summary>
        /// Customer-related results
        /// </summary>
        [Display(Name = "Customer")]
        Customer = 5,

        /// <summary>
        /// Employee-related results
        /// </summary>
        [Display(Name = "Employee")]
        Employee = 6,

        /// <summary>
        /// Process-related results
        /// </summary>
        [Display(Name = "Process")]
        Process = 7,

        /// <summary>
        /// Performance-related results
        /// </summary>
        [Display(Name = "Performance")]
        Performance = 8,

        /// <summary>
        /// Project-related results
        /// </summary>
        [Display(Name = "Project")]
        Project = 9,

        /// <summary>
        /// Strategic results related to strategic goals
        /// </summary>
        [Display(Name = "Strategic")]
        Strategic = 10,

        /// <summary>
        /// Other types of results
        /// </summary>
        [Display(Name = "Other")]
        Other = 11
    }

    /// <summary>
    /// Simplified indicator status for views
    /// </summary>
    public enum SimpleIndicatorStatus
    {
        /// <summary>
        /// Not started status
        /// </summary>
        [Display(Name = "Not Started")]
        NotStarted = 0,

        /// <summary>
        /// In progress status
        /// </summary>
        [Display(Name = "In Progress")]
        InProgress = 1,

        /// <summary>
        /// At risk status
        /// </summary>
        [Display(Name = "At Risk")]
        AtRisk = 2,

        /// <summary>
        /// Completed status
        /// </summary>
        [Display(Name = "Completed")]
        Completed = 3,

        /// <summary>
        /// Cancelled status
        /// </summary>
        [Display(Name = "Cancelled")]
        Cancelled = 4
    }
}