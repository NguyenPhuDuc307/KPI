namespace KPISolution.Models.ViewModels.Organization
{
    // ViewModel for assigning success factors to an objective
    public class ObjectiveAssignSuccessFactorsViewModel
    {
        public Guid ObjectiveId { get; init; }
        public string ObjectiveName { get; set; } = string.Empty; // Initialize to empty string

        // Using the correct ViewModel from the same namespace
        public List<ObjectiveSuccessFactorViewModel> AssignedSuccessFactors { get; init; } = [];
        public List<ObjectiveSuccessFactorViewModel> AvailableSuccessFactors { get; set; } = [];

        public List<Guid> SelectedSuccessFactorIds { get; init; } = [];
    }
}