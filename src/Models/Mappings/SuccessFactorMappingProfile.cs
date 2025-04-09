namespace KPISolution.Models.Mappings
{
    public class SuccessFactorMappingProfile : Profile
    {
        public SuccessFactorMappingProfile()
        {
            // Map từ ViewModel sang Entity
            CreateMap<SuccessFactorCreateViewModel, SuccessFactor>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.IsCritical, opt => opt.MapFrom(src => src.IsCritical))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dest => dest.ObjectiveId, opt => opt.MapFrom(src => src.ObjectiveId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate ?? DateTime.UtcNow))
                .ForMember(dest => dest.TargetDate, opt => opt.MapFrom(src => src.TargetDate ?? DateTime.UtcNow.AddMonths(3)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.ResultIndicators, opt => opt.Ignore())
                .ForMember(dest => dest.PerformanceIndicators, opt => opt.Ignore())
                .ForMember(dest => dest.Measurements, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.Objective, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUser, opt => opt.Ignore());

            // Map từ Entity sang ViewModel
            CreateMap<SuccessFactor, SuccessFactorViewModel>();

            // Map từ Entity sang chi tiết ViewModel
            CreateMap<SuccessFactor, SuccessFactorDetailsViewModel>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : string.Empty))
                .ForMember(dest => dest.ObjectiveName, opt => opt.MapFrom(src => src.Objective != null ? src.Objective.Name : string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.IndicatorCount, opt => opt.MapFrom(src => src.PerformanceIndicators != null ? src.PerformanceIndicators.Count : 0))
                .ForMember(dest => dest.Indicators, opt => opt.MapFrom(src => src.PerformanceIndicators))
                .ForMember(dest => dest.ResultIndicators, opt => opt.MapFrom(src => src.ResultIndicators))
                .ForMember(dest => dest.ProgressHistory, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt));

            // Map cho Edit ViewModel
            CreateMap<SuccessFactor, SuccessFactorEditViewModel>();
            CreateMap<SuccessFactorEditViewModel, SuccessFactor>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.ResultIndicators, opt => opt.Ignore())
                .ForMember(dest => dest.PerformanceIndicators, opt => opt.Ignore())
                .ForMember(dest => dest.Measurements, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.Objective, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsibleUser, opt => opt.Ignore());

            // Map cho List Item ViewModel
            CreateMap<SuccessFactor, SuccessFactorListItemViewModel>()
                .ForMember(dest => dest.ObjectiveName, opt => opt.MapFrom(src => src.Objective != null ? src.Objective.Name : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

            // Map cho Progress ViewModel
            CreateMap<SuccessFactor, SuccessFactorProgressViewModel>()
                .ForMember(dest => dest.SuccessFactorId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SuccessFactorName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.ProgressPercentage))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RiskLevel, opt => opt.MapFrom(src => src.RiskLevel));
        }
    }
}