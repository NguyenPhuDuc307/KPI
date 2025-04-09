namespace KPISolution.Models.Mappings
{
    public class PerformanceIndicatorMappingProfile : Profile
    {
        public PerformanceIndicatorMappingProfile()
        {
            CreateMap<PerformanceIndicator, IndicatorSummaryViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.LastMeasurementDate, opt => opt.MapFrom(src => src.Measurements != null && src.Measurements.Any() ? src.Measurements.Max(m => m.MeasurementDate) : (DateTime?)null))
                .ForMember(dest => dest.PercentageComplete, opt => opt.MapFrom(src => 
                    src.CurrentValue.HasValue && src.TargetValue.HasValue && src.TargetValue.Value != 0 
                        ? Math.Round((src.CurrentValue.Value / src.TargetValue.Value) * 100, 2)
                        : (decimal?)null))
                .ForMember(dest => dest.Trend, opt => opt.Ignore())
                .ForMember(dest => dest.TrendDisplay, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusCssClass, opt => opt.MapFrom(src => GetStatusCssClass(src.Status)));

            CreateMap<PerformanceIndicator, IndicatorViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue));

            CreateMap<PerformanceIndicatorCreateViewModel, PerformanceIndicator>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IndicatorStatus.Draft))
                .ForMember(dest => dest.SuccessFactorId, opt => opt.MapFrom(src => src.SuccessFactorId));

            CreateMap<PerformanceIndicator, PerformanceIndicatorListItemViewModel>()
                .ForMember(dest => dest.ResultIndicatorName, opt => opt.MapFrom(src => src.ResultIndicator != null ? src.ResultIndicator.Name : string.Empty))
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor != null ? src.SuccessFactor.Name : string.Empty));

            CreateMap<PerformanceIndicator, PerformanceIndicatorDetailsViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsKey))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(dest => dest.MinAlertThreshold, opt => opt.MapFrom(src => src.MinAlertThreshold))
                .ForMember(dest => dest.MaxAlertThreshold, opt => opt.MapFrom(src => src.MaxAlertThreshold))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.SuccessFactorId, opt => opt.MapFrom(src => src.SuccessFactorId != null ? src.SuccessFactorId : (src.ResultIndicator != null ? src.ResultIndicator.SuccessFactorId : Guid.Empty)))
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor != null ? src.SuccessFactor.Name : (src.ResultIndicator != null && src.ResultIndicator.SuccessFactor != null ? src.ResultIndicator.SuccessFactor.Name : string.Empty)))
                .ForMember(dest => dest.SuccessFactorIsCritical, opt => opt.MapFrom(src => (src.SuccessFactor != null && src.SuccessFactor.IsCritical) || (src.ResultIndicator != null && src.ResultIndicator.SuccessFactor != null && src.ResultIndicator.SuccessFactor.IsCritical)))
                .ForMember(dest => dest.MeasurementCount, opt => opt.MapFrom(src => src.Measurements != null ? src.Measurements.Count : 0))
                .ForMember(dest => dest.LastMeasurementDate, opt => opt.MapFrom(src => src.Measurements != null && src.Measurements.Any() ? src.Measurements.Max(m => m.MeasurementDate) : (DateTime?)null))
                .ForMember(dest => dest.RecentMeasurements, opt => opt.MapFrom(src => src.Measurements != null ? src.Measurements.OrderByDescending(m => m.MeasurementDate).Take(10).Select(m => new MeasurementViewModel
                {
                    Id = m.Id,
                    MeasurementDate = m.MeasurementDate,
                    Value = m.Value,
                    Status = m.Status,
                    Notes = m.Notes,
                    IndicatorId = src.Id,
                    IndicatorName = src.Name,
                    IndicatorType = src.IsKey ? "KPI" : "PI",
                    DepartmentName = src.Department != null ? src.Department.Name : "Unknown",
                    IndicatorUnit = src.Unit.ToString(),
                    TargetValue = src.TargetValue
                }) : new List<MeasurementViewModel>()));

            CreateMap<PerformanceIndicator, PerformanceIndicatorEditViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsKey))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinAlertThreshold, opt => opt.MapFrom(src => src.MinAlertThreshold))
                .ForMember(dest => dest.MaxAlertThreshold, opt => opt.MapFrom(src => src.MaxAlertThreshold))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => ParseEnum<MeasurementUnit>(src.Unit) ?? MeasurementUnit.Percentage))
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.MeasurementFrequency))
                .ForMember(dest => dest.ReviewFrequency, opt => opt.MapFrom(src => src.ReviewFrequency))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ParseEnum<ActivityType>(src.ActivityType)))
                .ForMember(dest => dest.ControlLevel, opt => opt.MapFrom(src => src.ControlLevel))
                .ForMember(dest => dest.ActionPlan, opt => opt.MapFrom(src => src.ActionPlan))
                .ForMember(dest => dest.DataCollectionMethod, opt => opt.MapFrom(src => ParseEnum<DataCollectionMethod>(src.DataCollectionMethod)))
                .ForMember(dest => dest.SuccessFactorId, opt => opt.MapFrom(src => src.SuccessFactorId))
                .ForMember(dest => dest.ResultIndicatorId, opt => opt.MapFrom(src => src.ResultIndicatorId))
                .ForMember(dest => dest.ResponsibleTeamMemberId, opt => opt.MapFrom(src => src.ResponsiblePersonId != null ? Guid.Parse(src.ResponsiblePersonId) : (Guid?)null));

            CreateMap<PerformanceIndicatorEditViewModel, PerformanceIndicator>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.IsKey, opt => opt.MapFrom(src => src.IsKey))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Formula))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.MinAlertThreshold, opt => opt.MapFrom(src => src.MinAlertThreshold))
                .ForMember(dest => dest.MaxAlertThreshold, opt => opt.MapFrom(src => src.MaxAlertThreshold))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()))
                .ForMember(dest => dest.MeasurementFrequency, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.ReviewFrequency, opt => opt.MapFrom(src => src.ReviewFrequency))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType.HasValue ? src.ActivityType.Value.ToString() : null))
                .ForMember(dest => dest.ControlLevel, opt => opt.MapFrom(src => src.ControlLevel))
                .ForMember(dest => dest.ActionPlan, opt => opt.MapFrom(src => src.ActionPlan))
                .ForMember(dest => dest.DataCollectionMethod, opt => opt.MapFrom(src => src.DataCollectionMethod.HasValue ? src.DataCollectionMethod.Value.ToString() : null))
                .ForMember(dest => dest.SuccessFactorId, opt => opt.MapFrom(src => src.SuccessFactorId))
                .ForMember(dest => dest.ResultIndicatorId, opt => opt.MapFrom(src => src.ResultIndicatorId))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src => src.ResponsibleTeamMemberId.HasValue ? src.ResponsibleTeamMemberId.Value.ToString() : null));

            CreateMap<Measurement, MeasurementViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MeasurementDate, opt => opt.MapFrom(src => src.MeasurementDate))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        }

        private static string GetStatusCssClass(IndicatorStatus status)
        {
            return status switch
            {
                IndicatorStatus.OnTarget => "bg-success",
                IndicatorStatus.AtRisk => "bg-warning",
                IndicatorStatus.BelowTarget => "bg-danger",
                IndicatorStatus.Draft => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        private static TEnum? ParseEnum<TEnum>(string? value) where TEnum : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return Enum.TryParse<TEnum>(value, true, out var result) ? result : null;
        }
    }
}