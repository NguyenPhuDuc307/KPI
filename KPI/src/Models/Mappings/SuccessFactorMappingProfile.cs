using AutoMapper;
using KPISolution.Models.Entities.Objective;
using KPISolution.Models.ViewModels.SuccessFactor;

namespace KPISolution.Models.Mappings
{
    public class SuccessFactorMappingProfile : Profile
    {
        public SuccessFactorMappingProfile()
        {
            // Entity to view model mappings
            CreateMap<SuccessFactor, SuccessFactorDetailsViewModel>()
                .ForMember(dest => dest.CriticalSuccessFactors, opt => opt.Ignore()) // This will be populated separately
                .ForMember(dest => dest.BusinessObjectiveName, opt => opt.Ignore()) // This will be populated separately
                .ForMember(dest => dest.DepartmentName, opt => opt.Ignore()); // This will be populated separately

            // View model to entity mappings
            CreateMap<SuccessFactorEditViewModel, SuccessFactor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set separately for new entities
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.BusinessObjective, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.CriticalSuccessFactors, opt => opt.Ignore());
        }
    }
}