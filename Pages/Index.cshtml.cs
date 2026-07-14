using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RevelioII.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RevelioII.Services.IGraphManagementService _graphManagementService;

        public IndexModel(ILogger<IndexModel> logger, RevelioII.Services.IGraphManagementService graphManagementService)
        {
            _logger = logger;
            _graphManagementService = graphManagementService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnGetGraphDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var graph = await _graphManagementService.GetGraphViewAsync(cancellationToken);
                return new JsonResult(graph);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load graph data for the home page.");
                Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
                return new JsonResult(new { error = "Unable to load graph data." });
            }
        }
    }
}
