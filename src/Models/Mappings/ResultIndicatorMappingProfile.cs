namespace KPISolution.Models.Mappings
{
    public class ResultIndicatorMappingProfile : Profile
    {
        public ResultIndicatorMappingProfile()
        {
            CreateMap<ResultIndicatorCreateViewModel, ResultIndicator>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IndicatorStatus.Draft))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src =>
                    src.ResponsibleUserId.HasValue ? src.ResponsibleUserId.Value.ToString() : null));

            CreateMap<ResultIndicator, ResultIndicatorListItemViewModel>()
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor.Name))
                .ForMember(dest => dest.SuccessFactorIsCritical, opt => opt.MapFrom(src => src.SuccessFactor.IsCritical));

            CreateMap<ResultIndicator, ResultIndicatorDetailsViewModel>()
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.SuccessFactor.Name))
                .ForMember(dest => dest.SuccessFactorIsCritical, opt => opt.MapFrom(src => src.SuccessFactor.IsCritical))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src => src.ResponsiblePersonId))
                .ForMember(dest => dest.ResponsiblePersonName, opt => opt.MapFrom(src => src.ResponsiblePerson != null ? $"{src.ResponsiblePerson.FirstName} {src.ResponsiblePerson.LastName}" : null))
                .ForMember(dest => dest.PerformanceIndicators, opt => opt.MapFrom(src => src.PerformanceIndicators));

            CreateMap<ResultIndicator, ResultIndicatorEditViewModel>()
                .ForMember(dest => dest.ResponsibleUserId, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.ResponsiblePersonId) ? Guid.Parse(src.ResponsiblePersonId) : (Guid?)null));

            CreateMap<ResultIndicatorEditViewModel, ResultIndicator>()
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src =>
                    src.ResponsibleUserId.HasValue ? src.ResponsibleUserId.Value.ToString() : null));
        }
    }
}