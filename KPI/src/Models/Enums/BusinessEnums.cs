namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Business areas for Key Risk Indicators
    /// </summary>
    public enum BusinessArea
    {
        Financial = 1,
        Customer = 2,
        Operations = 3,
        MarketingSales = 4,
        HumanResources = 5,
        InformationTechnology = 6,
        ResearchDevelopment = 7,
        SupplyChain = 8,
        ESG = 9,
        ComplianceRisk = 10,
        Strategic = 11,
        Other = 99
    }

    /// <summary>
    /// Activity types for Performance Indicators
    /// </summary>
    public enum ActivityType
    {
        StandardOperations = 1,
        ProcessImprovement = 2,
        CostSaving = 3,
        QualityImprovement = 4,
        Efficiency = 5,
        Innovation = 6,
        CustomerService = 7,
        EmployeeDevelopment = 8,
        PerformanceOptimization = 9,
        RiskMitigation = 10,
        Compliance = 11,
        Project = 12,
        Strategic = 13,
        Other = 99
    }

    /// <summary>
    /// Process areas for Result Indicators
    /// </summary>
    public enum ProcessArea
    {
        CoreBusiness = 1,
        Support = 2,
        Management = 3,
        Production = 4,
        Sales = 5,
        ServiceDelivery = 6,
        CustomerRelationship = 7,
        ProductDevelopment = 8,
        Distribution = 9,
        SupplyChain = 10,
        QualityAssurance = 11,
        Marketing = 12,
        IT = 13,
        HR = 14,
        Financial = 15,
        Other = 99
    }

    /// <summary>
    /// Impact levels for Key Risk Indicators
    /// </summary>
    public enum ImpactLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
} 