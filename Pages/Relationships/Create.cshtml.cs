using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Relationships
{
    public class CreateModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public CreateModel(IGraphManagementService service)
        {
            _service = service;
        }

        [BindProperty]
        public Relationship GraphRelationship { get; set; } = default!;

        public SelectList NodesList { get; set; } = default!;

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var nodes = await _service.GetAllNodesAsync(cancellationToken);
            NodesList = new SelectList(nodes, "Id", "Label");
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var nodes = await _service.GetAllNodesAsync(cancellationToken);
                NodesList = new SelectList(nodes, "Id", "Label");
                return Page();
            }

            await _service.CreateRelationshipAsync(GraphRelationship, cancellationToken);
            return RedirectToPage("./Index");
        }
    }
}
