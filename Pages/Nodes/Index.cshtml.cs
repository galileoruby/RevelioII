using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Nodes
{
    public class IndexModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public IndexModel(IGraphManagementService service)
        {
            _service = service;
        }

        public IEnumerable<Node> Nodes { get; set; } = new List<Node>();

        public async Task OnGetAsync()
        {
            Nodes = await _service.GetAllNodesAsync();
        }
    }
}
