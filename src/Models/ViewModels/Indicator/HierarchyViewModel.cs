using KPISolution.Models.Entities.Indicator;
using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.Indicator
{
    public class HierarchyViewModel
    {
        public IEnumerable<KPISolution.Models.Entities.Indicator.ResultIndicator> ResultIndicators { get; set; } = new List<KPISolution.Models.Entities.Indicator.ResultIndicator>();
        public IEnumerable<KPISolution.Models.Entities.Indicator.SuccessFactor> SuccessFactors { get; set; } = new List<KPISolution.Models.Entities.Indicator.SuccessFactor>();
    }
}