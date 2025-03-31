using AutoMapper;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.ViewModels.CSF;
using System;
using System.Collections.Generic;
using System.Linq;
using RiskLevel = KPISolution.Models.Enums.RiskLevel;
using CSFStatus = KPISolution.Models.Enums.CSFStatus;

namespace KPISolution.Models.ViewModels.CSF.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping CSF entities to view models
    /// </summary>
    public class CsfMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsfMappingProfile"/> class.
        /// Creates mappings for CSF entities and view models
        /// </summary>
        public CsfMappingProfile()
        {
            // CSF -> CsfDetailsViewModel
            CreateMap<CriticalSuccessFactor, CsfDetailsViewModel>()
                .ForMember(dest => dest.BusinessObjectiveName, opt => opt.MapFrom(src => src.BusinessObjective != null ? src.BusinessObjective.Name : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.PriorityDisplay, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CategoryDisplay, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.RiskLevelDisplay, opt => opt.MapFrom(src => src.RiskLevel.ToString()))
                .ForMember(dest => dest.DaysRemaining, opt => opt.Ignore()) // Calculate this separately
                .ForMember(dest => dest.TimeElapsedPercentage, opt => opt.Ignore()) // Calculate this separately
                .ForMember(dest => dest.IsOnTrack, opt => opt.Ignore()) // Calculate this separately
                .ForMember(dest => dest.StatusCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.RiskLevelCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.ProgressCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.LinkedKpis, opt => opt.Ignore()) // Map this separately
                .ForMember(dest => dest.ProgressUpdates, opt => opt.Ignore()); // Map this separately

            // CSF -> EditCsfViewModel
            CreateMap<CriticalSuccessFactor, EditCsfViewModel>()
                .ForMember(dest => dest.SelectedKpiIds, opt => opt.MapFrom(src =>
                    src.CSFKPIs != null ? src.CSFKPIs.Select(k => k.KpiId).ToList() : new List<Guid>()))
                .ForMember(dest => dest.LinkedKpis, opt => opt.Ignore()) // Map this separately 
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore()) // Set this separately if needed
                .ForMember(dest => dest.BusinessObjectives, opt => opt.Ignore())
                .ForMember(dest => dest.Departments, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableKpis, opt => opt.Ignore())
                .ForMember(dest => dest.RecentProgressUpdates, opt => opt.Ignore());

            // CSF -> CsfListItemViewModel
            CreateMap<CriticalSuccessFactor, CsfListItemViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.PriorityDisplay, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CategoryDisplay, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.RiskLevelDisplay, opt => opt.MapFrom(src => src.RiskLevel.ToString()))
                .ForMember(dest => dest.LinkedKpisCount, opt => opt.MapFrom(src => src.CSFKPIs != null ? src.CSFKPIs.Count : 0))
                .ForMember(dest => dest.StatusCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.RiskLevelCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.ProgressCssClass, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.IsOnTrack, opt => opt.Ignore()) // Calculate this separately
                .ForMember(dest => dest.DaysRemaining, opt => opt.Ignore()) // Calculate this separately
                .ForMember(dest => dest.NeedsAttention, opt => opt.Ignore()); // Calculate this separately

            // CreateCsfViewModel -> CSF
            CreateMap<CreateCsfViewModel, CriticalSuccessFactor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CSFKPIs, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressUpdates, opt => opt.Ignore())
                .ForMember(dest => dest.BusinessObjective, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore());

            // EditCsfViewModel -> CSF
            CreateMap<EditCsfViewModel, CriticalSuccessFactor>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CSFKPIs, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressUpdates, opt => opt.Ignore())
                .ForMember(dest => dest.BusinessObjective, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore());

            // CSFKPI -> LinkedKpiViewModel
            CreateMap<CSFKPI, LinkedKpiViewModel>()
                .ForMember(dest => dest.KpiId, opt => opt.MapFrom(src => src.KpiId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.Name : string.Empty))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.Code : string.Empty))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.TargetValue : 0))
                .ForMember(dest => dest.CurrentValue, opt => opt.Ignore()) // Set this separately if needed
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.Unit : null))
                .ForMember(dest => dest.RelationshipStrengthDisplay, opt => opt.MapFrom(src => src.RelationshipStrength.ToString()))
                .ForMember(dest => dest.ImpactLevelDisplay, opt => opt.MapFrom(src => src.ImpactLevel.ToString()))
                .ForMember(dest => dest.PerformanceCssClass, opt => opt.Ignore()); // Set this separately

            // CSFKPI -> CsfKpiRelationshipViewModel
            CreateMap<CSFKPI, CsfKpiRelationshipViewModel>()
                .ForMember(dest => dest.KpiId, opt => opt.MapFrom(src => src.KpiId))
                .ForMember(dest => dest.KpiName, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.Name : string.Empty))
                .ForMember(dest => dest.KpiCode, opt => opt.MapFrom(src => src.KPI != null ? src.KPI.Code : string.Empty))
                .ForMember(dest => dest.KpiType, opt => opt.MapFrom(src => src.KpiType));

            // CSFProgress -> CsfProgressViewModel
            CreateMap<CSFProgress, CsfProgressViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.ProgressPercentage))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.IsAtRisk, opt => opt.MapFrom(src => src.RiskLevel == RiskLevel.High || src.RiskLevel == RiskLevel.Critical))
                .ForMember(dest => dest.IsBehindSchedule, opt => opt.MapFrom(src => src.Status == CSFStatus.Delayed || src.Status == CSFStatus.AtRisk))
                .ForMember(dest => dest.Blockers, opt => opt.MapFrom(src => src.Challenges))
                .ForMember(dest => dest.MitigationActions, opt => opt.MapFrom(src => src.NextSteps));

            // CSFProgress -> CsfProgressUpdateViewModel
            CreateMap<CSFProgress, CsfProgressUpdateViewModel>()
                .ForMember(dest => dest.CSFId, opt => opt.MapFrom(src => src.CSFId))
                .ForMember(dest => dest.CSFName, opt => opt.Ignore()) // Set this separately from the CSF entity
                .ForMember(dest => dest.CurrentProgressPercentage, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.CurrentStatus, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.CurrentRiskLevel, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.StatusOptions, opt => opt.Ignore()) // Set this separately
                .ForMember(dest => dest.RiskLevelOptions, opt => opt.Ignore()); // Set this separately

            // CSFProgress -> CsfProgressHistoryViewModel
            CreateMap<CSFProgress, CsfProgressHistoryViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.ProgressPercentage))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.RiskLevel, opt => opt.MapFrom(src => src.RiskLevel))
                .ForMember(dest => dest.RiskLevelDisplay, opt => opt.MapFrom(src => src.RiskLevel.ToString()))
                .ForMember(dest => dest.Achievements, opt => opt.MapFrom(src => src.Achievements))
                .ForMember(dest => dest.Challenges, opt => opt.MapFrom(src => src.Challenges))
                .ForMember(dest => dest.NextSteps, opt => opt.MapFrom(src => src.NextSteps))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.NeedsAttention, opt => opt.MapFrom(src => src.NeedsAttention))
                .ForMember(dest => dest.ExpectedCompletionDate, opt => opt.MapFrom(src => src.ExpectedCompletionDate))
                .ForMember(dest => dest.PreviousProgressPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressChange, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousStatus, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousStatusDisplay, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousRiskLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressChangeCssClass, opt => opt.Ignore());
        }
    }
}