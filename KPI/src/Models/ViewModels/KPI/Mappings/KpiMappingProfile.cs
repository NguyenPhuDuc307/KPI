using AutoMapper;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.ViewModels.KPI;
using KPISolution.Models.Entities.CSF;

namespace KPISolution.Models.ViewModels.KPI.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping KPI entities to view models
    /// </summary>
    public class KpiMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KpiMappingProfile"/> class.
        /// </summary>
        public KpiMappingProfile()
        {
            // Mapping from KpiBase to KpiListItemViewModel
            CreateMap<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            // Add mapping from KpiValue to KpiValueViewModel
            CreateMap<KpiValue, KpiValueViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.KpiId, opt => opt.MapFrom(src => src.KpiId))
                .ForMember(dest => dest.ActualValue, opt => opt.MapFrom(src => src.ActualValue))
                .ForMember(dest => dest.MeasurementDate, opt => opt.MapFrom(src => src.MeasurementDate))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.KpiType, opt => opt.Ignore())
                .ForMember(dest => dest.TargetValue, opt => opt.Ignore())
                .ForMember(dest => dest.Period, opt => opt.Ignore())
                .ForMember(dest => dest.DataSource, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AchievementPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.Trend, opt => opt.Ignore())
                .ForMember(dest => dest.Variance, opt => opt.Ignore())
                .ForMember(dest => dest.StatusCssClass, opt => opt.Ignore());

            // Specific mapping for KRI
            CreateMap<KRI, KpiListItemViewModel>()
                .IncludeBase<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.StrategicObjective, opt => opt.MapFrom(src => src.StrategicObjective))
                .ForMember(dest => dest.ExecutiveOwner, opt => opt.MapFrom(src => src.ExecutiveOwner))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.KeyResultIndicator));

            // Specific mapping for PI
            CreateMap<PI, KpiListItemViewModel>()
                .IncludeBase<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.ToString()))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.ResponsiblePerson))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src =>
                    src.IsKey ? Models.Enums.KpiType.StandaloneKPI : Models.Enums.KpiType.PerformanceIndicator));

            // Specific mapping for RI
            CreateMap<RI, KpiListItemViewModel>()
                .IncludeBase<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.ParentKpiCode, opt => opt.MapFrom(src => src.ParentKRI != null ? src.ParentKRI.Code : null))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator));

            // Specific mapping for StandaloneKPI
            CreateMap<Models.Entities.KPI.KPI, KpiListItemViewModel>()
                .IncludeBase<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.StandaloneKPI));

            // Base mapping for KpiBase to KpiDetailsViewModel
            CreateMap<KpiBase, KpiDetailsViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinThreshold, opt => opt.MapFrom(src => src.MinimumValue))
                .ForMember(dest => dest.MaxThreshold, opt => opt.MapFrom(src => src.MaximumValue))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.FrequencyString, opt => opt.MapFrom(src => src.Frequency.ToString()))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.MeasurementDirection))
                .ForMember(dest => dest.DirectionString, opt => opt.MapFrom(src => src.MeasurementDirection.ToString()))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.ResponsiblePerson))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Department, opt => opt.Ignore());

            // KRI specific mapping to KpiDetailsViewModel
            CreateMap<KRI, KpiDetailsViewModel>()
                .IncludeBase<KpiBase, KpiDetailsViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.KeyResultIndicator))
                .ForMember(dest => dest.KpiTypeString, opt => opt.MapFrom(src => Models.Enums.KpiType.KeyResultIndicator.ToString()))
                .ForMember(dest => dest.StrategicObjective, opt => opt.MapFrom(src => src.StrategicObjective))
                .ForMember(dest => dest.ImpactLevel, opt => opt.MapFrom(src => src.ImpactLevel))
                .ForMember(dest => dest.ExecutiveOwner, opt => opt.MapFrom(src => src.ExecutiveOwner))
                .ForMember(dest => dest.ConfidenceLevel, opt => opt.MapFrom(src => src.ConfidenceLevel));

            // PI specific mapping to KpiDetailsViewModel
            CreateMap<PI, KpiDetailsViewModel>()
                .IncludeBase<KpiBase, KpiDetailsViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src =>
                    src.IsKey ? Models.Enums.KpiType.StandaloneKPI : Models.Enums.KpiType.PerformanceIndicator))
                .ForMember(dest => dest.KpiTypeString, opt => opt.MapFrom(src =>
                    src.IsKey ? Models.Enums.KpiType.StandaloneKPI.ToString() : Models.Enums.KpiType.PerformanceIndicator.ToString()))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType));

            // RI specific mapping to KpiDetailsViewModel
            CreateMap<RI, KpiDetailsViewModel>()
                .IncludeBase<KpiBase, KpiDetailsViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator))
                .ForMember(dest => dest.KpiTypeString, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator.ToString()))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea))
                .ForMember(dest => dest.MeasurementScope, opt => opt.MapFrom(src => src.MeasurementScope))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.TimeFrame))
                .ForMember(dest => dest.ResultType, opt => opt.MapFrom(src => src.ResultType))
                .ForMember(dest => dest.ContributionPercentage, opt => opt.MapFrom(src => src.ContributionPercentage));

            // StandaloneKPI specific mapping to KpiDetailsViewModel
            CreateMap<Models.Entities.KPI.KPI, KpiDetailsViewModel>()
                .IncludeBase<KpiBase, KpiDetailsViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.StandaloneKPI))
                .ForMember(dest => dest.KpiTypeString, opt => opt.MapFrom(src => Models.Enums.KpiType.StandaloneKPI.ToString()));

            // Map KpiBase to LinkedKpiViewModel for basic properties
            CreateMap<KpiBase, LinkedKpiViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department));

            // Specific mapping for KRI to LinkedKpiViewModel
            CreateMap<KRI, LinkedKpiViewModel>()
                .IncludeBase<KpiBase, LinkedKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.KeyResultIndicator));

            // Specific mapping for RI to LinkedKpiViewModel
            CreateMap<RI, LinkedKpiViewModel>()
                .IncludeBase<KpiBase, LinkedKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator));

            // Specific mapping for PI to LinkedKpiViewModel
            CreateMap<PI, LinkedKpiViewModel>()
                .IncludeBase<KpiBase, LinkedKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src =>
                    src.IsKey ? Models.Enums.KpiType.StandaloneKPI : Models.Enums.KpiType.PerformanceIndicator));

            // Map CriticalSuccessFactor to LinkedCsfViewModel
            CreateMap<CriticalSuccessFactor, LinkedCsfViewModel>()
                .ForMember(dest => dest.CsfId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.RelationshipStrength, opt => opt.Ignore())
                .ForMember(dest => dest.ImpactLevel, opt => opt.Ignore());

            // Map KpiBase to EditKpiViewModel for basic properties
            CreateMap<KpiBase, EditKpiViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinimumValue, opt => opt.MapFrom(src => src.MinimumValue))
                .ForMember(dest => dest.MaximumValue, opt => opt.MapFrom(src => src.MaximumValue))
                .ForMember(dest => dest.MeasurementFrequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.ResponsiblePerson))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.MeasurementDirection, opt => opt.MapFrom(src => src.MeasurementDirection))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit));

            // KRI to EditKpiViewModel mapping
            CreateMap<KRI, EditKpiViewModel>()
                .IncludeBase<KpiBase, EditKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.KeyResultIndicator))
                .ForMember(dest => dest.StrategicObjective, opt => opt.MapFrom(src => src.StrategicObjective))
                .ForMember(dest => dest.ImpactLevel, opt => opt.MapFrom(src => src.ImpactLevel))
                .ForMember(dest => dest.BusinessArea, opt => opt.MapFrom(src => src.BusinessArea))
                .ForMember(dest => dest.ExecutiveOwner, opt => opt.MapFrom(src => src.ExecutiveOwner))
                .ForMember(dest => dest.ConfidenceLevel, opt => opt.MapFrom(src => src.ConfidenceLevel));

            // RI to EditKpiViewModel mapping
            CreateMap<RI, EditKpiViewModel>()
                .IncludeBase<KpiBase, EditKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator))
                .ForMember(dest => dest.ParentKriId, opt => opt.MapFrom(src => src.ParentKriId))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea))
                .ForMember(dest => dest.ResponsibleManager, opt => opt.MapFrom(src => src.ResponsibleManager))
                .ForMember(dest => dest.MeasurementScope, opt => opt.MapFrom(src => src.MeasurementScope))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.TimeFrame))
                .ForMember(dest => dest.ResultType, opt => opt.MapFrom(src => src.ResultType))
                .ForMember(dest => dest.ContributionPercentage, opt => opt.MapFrom(src => src.ContributionPercentage))
                .ForMember(dest => dest.CalculationMethod, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource));

            // PI to EditKpiViewModel mapping
            CreateMap<PI, EditKpiViewModel>()
                .IncludeBase<KpiBase, EditKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.PerformanceIndicator))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType));

            // StandaloneKPI to EditKpiViewModel mapping
            CreateMap<Models.Entities.KPI.KPI, EditKpiViewModel>()
                .IncludeBase<KpiBase, EditKpiViewModel>()
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.StandaloneKPI));

            // Map from EditKpiViewModel to RI (for updating)
            CreateMap<EditKpiViewModel, RI>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.MeasurementUnit ?? src.Unit))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinimumValue, opt => opt.MapFrom(src => src.MinimumValue))
                .ForMember(dest => dest.MaximumValue, opt => opt.MapFrom(src => src.MaximumValue))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.MeasurementFrequency))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.MeasurementDirection, opt => opt.MapFrom(src => src.MeasurementDirection))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea != null ? src.ProcessArea.Value : Models.Enums.ProcessArea.Other))
                .ForMember(dest => dest.ResponsibleManager, opt => opt.MapFrom(src => src.ResponsibleManager))
                .ForMember(dest => dest.MeasurementScope, opt => opt.MapFrom(src => src.MeasurementScope))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.TimeFrame))
                .ForMember(dest => dest.ResultType, opt => opt.MapFrom(src => src.ResultType))
                .ForMember(dest => dest.ContributionPercentage, opt => opt.MapFrom(src => src.ContributionPercentage))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.CalculationMethod ?? src.Formula))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsRIKey));

            // Map CreateKpiViewModel to KpiBase
            CreateMap<CreateKpiViewModel, KpiBase>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinimumValue, opt => opt.MapFrom(src => src.MinimumValue))
                .ForMember(dest => dest.MaximumValue, opt => opt.MapFrom(src => src.MaximumValue))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.MeasurementFrequency))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.MeasurementDirection, opt => opt.MapFrom(src => src.MeasurementDirection))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula));

            // Map CreateKpiViewModel to KRI
            CreateMap<CreateKpiViewModel, KRI>()
                .IncludeBase<CreateKpiViewModel, KpiBase>()
                .ForMember(dest => dest.StrategicObjective, opt => opt.MapFrom(src => src.StrategicObjective))
                .ForMember(dest => dest.ImpactLevel, opt => opt.MapFrom(src => src.ImpactLevel))
                .ForMember(dest => dest.BusinessArea, opt => opt.MapFrom(src => src.BusinessArea))
                .ForMember(dest => dest.ExecutiveOwner, opt => opt.MapFrom(src => src.ExecutiveOwner))
                .ForMember(dest => dest.ConfidenceLevel, opt => opt.MapFrom(src => src.ConfidenceLevel));

            // Map CreateKpiViewModel to RI
            CreateMap<CreateKpiViewModel, RI>()
                .IncludeBase<CreateKpiViewModel, KpiBase>()
                .ForMember(dest => dest.ParentKriId, opt => opt.MapFrom(src => src.ParentKriId))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea != null ? src.ProcessArea.Value : Models.Enums.ProcessArea.Other))
                .ForMember(dest => dest.ResponsibleManager, opt => opt.MapFrom(src => src.ResponsibleManager))
                .ForMember(dest => dest.MeasurementScope, opt => opt.MapFrom(src => src.MeasurementScope))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.TimeFrame))
                .ForMember(dest => dest.ResultType, opt => opt.MapFrom(src => src.ResultType))
                .ForMember(dest => dest.ContributionPercentage, opt => opt.MapFrom(src => src.ContributionPercentage))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsRIKey));

            // Map CreateKpiViewModel to PI
            CreateMap<CreateKpiViewModel, PI>()
                .IncludeBase<CreateKpiViewModel, KpiBase>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType ?? Models.Enums.ActivityType.StandardOperations))
                .ForMember(dest => dest.PerformanceLevel, opt => opt.MapFrom(src => 3))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsPIKey))
                .ForMember(dest => dest.ActionPlan, opt => opt.MapFrom(src => src.ActionPlan))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.CriticalSuccessFactorId, opt => opt.Ignore())
                .ForMember(dest => dest.RIId, opt => opt.MapFrom(src => src.RIId));

            // Map CreateKpiViewModel to StandaloneKPI
            CreateMap<CreateKpiViewModel, Models.Entities.KPI.KPI>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.MeasurementDirection, opt => opt.MapFrom(src => src.MeasurementDirection))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate));
        }
    }
}