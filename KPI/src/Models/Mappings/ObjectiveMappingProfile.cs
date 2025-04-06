namespace KPISolution.Models.Mappings
{
    /// <summary>
    /// Cấu hình mapping cho Objective entities và ViewModels
    /// </summary>
    public class ObjectiveMappingProfile : Profile
    {
        public ObjectiveMappingProfile()
        {
            // Map từ Objective entity sang ObjectiveViewModel
            CreateMap<Objective, ObjectiveViewModel>()
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.TargetDate))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.ResponsiblePersonId ?? string.Empty))
                .ForMember(dest => dest.Perspective, opt => opt.MapFrom(src => src.BusinessPerspective))
                .ForMember(dest => dest.ParentObjectiveName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : string.Empty));

            // Map từ Objective entity sang ObjectiveListItemViewModel
            CreateMap<Objective, ObjectiveListItemViewModel>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.TargetDate))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.ResponsiblePersonId ?? string.Empty))
                .ForMember(dest => dest.Perspective, opt => opt.MapFrom(src => src.BusinessPerspective))
                .ForMember(dest => dest.HasChildren, opt => opt.MapFrom(src => src.Children != null && src.Children.Any()))
                .ForMember(dest => dest.TimeframeType, opt => opt.MapFrom(src => src.Timeframe));

            // Map từ Objective entity sang ObjectiveDetailsViewModel
            CreateMap<Objective, ObjectiveDetailsViewModel>()
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.TargetDate))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));

            // Map từ Objective entity sang ObjectiveTreeNodeViewModel
            CreateMap<Objective, ObjectiveTreeNodeViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));

            // Map từ Objective entity sang ObjectiveSummaryViewModel
            CreateMap<Objective, ObjectiveSummaryViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.ProgressCssClass, opt => opt.Ignore());

            // Map từ Objective entity sang ObjectiveCreateViewModel
            CreateMap<Objective, ObjectiveCreateViewModel>()
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.TargetDate))
                .ForMember(dest => dest.Perspective, opt => opt.MapFrom(src => src.BusinessPerspective))
                .ForMember(dest => dest.ParentObjectiveId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.Ignore());

            // Map từ ObjectiveCreateViewModel sang Objective entity
            CreateMap<ObjectiveCreateViewModel, Objective>()
                .ForMember(dest => dest.TargetDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.BusinessPerspective, opt => opt.MapFrom(src => src.Perspective))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentObjectiveId))
                .ForMember(dest => dest.ResponsiblePersonId, opt => opt.MapFrom(src => src.ResponsiblePersonId.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.SuccessFactors, opt => opt.Ignore());

            // Map từ SuccessFactor sang ObjectiveSuccessFactorViewModel
            CreateMap<SuccessFactor, ObjectiveSuccessFactorViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.IsCritical ? "Yếu tố cốt lõi" : "Yếu tố thường"))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.ProgressPercentage))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}