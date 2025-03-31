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
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.PerformanceIndicator));

            // Specific mapping for RI
            CreateMap<RI, KpiListItemViewModel>()
                .IncludeBase<KpiBase, KpiListItemViewModel>()
                .ForMember(dest => dest.ParentKpiCode, opt => opt.MapFrom(src => src.ParentKRI != null ? src.ParentKRI.Code : null))
                .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.ResultIndicator));

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
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => Models.Enums.KpiType.PerformanceIndicator))
                .ForMember(dest => dest.KpiTypeString, opt => opt.MapFrom(src => Models.Enums.KpiType.PerformanceIndicator.ToString()))
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
        }
    }
}