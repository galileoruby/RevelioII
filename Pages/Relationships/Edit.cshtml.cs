using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Relationships
{
    public class EditModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public EditModel(IGraphManagementService service)
        {
            _service = service;
        }

        [BindProperty]
        public Relationship GraphRelationship { get; set; } = default!;

        public SelectList NodesList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationship = await _service.GetRelationshipAsync(id.Value, cancellationToken);
            if (relationship == null)
            {
                return NotFound();
            }

            GraphRelationship = relationship;
            await LoadNodesAsync(cancellationToken);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await LoadNodesAsync(cancellationToken);
                return Page();
            }

            await _service.UpdateRelationshipAsync(GraphRelationship, cancellationToken);
            return RedirectToPage("./Index");
        }

        private async Task LoadNodesAsync(CancellationToken cancellationToken)
        {
            var nodes = await _service.GetAllNodesAsync(cancellationToken);
            NodesList = new SelectList(nodes, "Id", "Label");
        }
    }
}
