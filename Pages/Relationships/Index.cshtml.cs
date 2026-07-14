using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Relationships
{
    public class IndexModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public IndexModel(IGraphManagementService service)
        {
            _service = service;
        }

        public IEnumerable<Relationship> Relationships { get; set; } = new List<Relationship>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Relationships = await _service.GetAllRelationshipsAsync(cancellationToken);
        }
    }
}
