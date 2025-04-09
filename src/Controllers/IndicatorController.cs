namespace KPISolution.Controllers
{
    [Authorize]
    public class IndicatorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IndicatorController> _logger;

        public IndicatorController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<IndicatorController> logger)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Hierarchy()
        {
            try
            {
                var resultIndicators = await this._unitOfWork.ResultIndicators
                    .Include(ri => ri.PerformanceIndicators)
                    .Include(ri => ri.SuccessFactor)
                    .OrderBy(ri => ri.Name)
                    .ToListAsync();

                var successFactors = await this._unitOfWork.SuccessFactors
                    .Include(sf => sf.ResultIndicators)
                    .OrderBy(sf => sf.Name)
                    .ToListAsync();

                var model = new HierarchyViewModel
                {
                    ResultIndicators = resultIndicators,
                    SuccessFactors = successFactors
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving indicator hierarchy");
                return this.StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}