namespace KPISolution.Models.Mappings
{
    public class ResultIndicatorMappingProfile : Profile
    {
        public ResultIndicatorMappingProfile()
        {
            this.CreateMap<ResultIndicatorCreateViewModel, ResultIndicator>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IndicatorStatus.Draft))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src =>
                    src.ResponsibleUserId.HasValue ? src.ResponsibleUserId.Value.ToString() : null));

            this.CreateMap<ResultIndicator, ResultIndicatorListItemViewModel>()
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor != null ? src.SuccessFactor.Name : string.Empty))
                .ForMember(dest => dest.SuccessFactorIsCritical, opt => opt.MapFrom(src => src.SuccessFactor != null && src.SuccessFactor.IsCritical));

            this.CreateMap<Measurement, MeasurementViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MeasurementDate, opt => opt.MapFrom(src => src.MeasurementDate))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

            this.CreateMap<ResultIndicator, ResultIndicatorDetailsViewModel>()
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor != null ? src.SuccessFactor.Name : string.Empty))
                .ForMember(dest => dest.SuccessFactorIsCritical, opt => opt.MapFrom(src => src.SuccessFactor != null && src.SuccessFactor.IsCritical))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src => src.ResponsiblePersonId))
                .ForMember(dest => dest.ResponsiblePersonName, opt => opt.MapFrom(src => src.ResponsiblePerson != null ? $"{src.ResponsiblePerson.FirstName} {src.ResponsiblePerson.LastName}" : null))
                .ForMember(dest => dest.PerformanceIndicators, opt => opt.MapFrom(src => src.PerformanceIndicators))
                .ForMember(dest => dest.RecentMeasurements, opt => opt.MapFrom(src => src.Measurements != null ?
                    src.Measurements.OrderByDescending(m => m.MeasurementDate).Take(10) : new List<Measurement>()))
                .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src =>
                    src.Measurements != null && src.Measurements.Any()
                        ? src.Measurements.OrderByDescending(m => m.MeasurementDate)
                            .Select(m => m.Value)
                            .FirstOrDefault()
                        : (decimal?)null))
                .ForMember(dest => dest.MeasurementCount, opt => opt.MapFrom(src => src.Measurements != null ? src.Measurements.Count : 0))
                .ForMember(dest => dest.LastMeasurementDate, opt => opt.MapFrom(src =>
                    src.Measurements != null && src.Measurements.Any()
                        ? src.Measurements.Max(m => m.MeasurementDate)
                        : (DateTime?)null));

            this.CreateMap<ResultIndicator, ResultIndicatorEditViewModel>()
                .ForMember(dest => dest.ResponsibleUserId, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.ResponsiblePersonId) ? Guid.Parse(src.ResponsiblePersonId) : (Guid?)null));

            this.CreateMap<ResultIndicatorEditViewModel, ResultIndicator>()
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src =>
                    src.ResponsibleUserId.HasValue ? src.ResponsibleUserId.Value.ToString() : null));
        }
    }
}