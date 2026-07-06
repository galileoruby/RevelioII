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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationship = await _service.GetRelationshipAsync(id.Value);
            if (relationship == null)
            {
                return NotFound();
            }

            GraphRelationship = relationship;
            await LoadNodesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadNodesAsync();
                return Page();
            }

            await _service.UpdateRelationshipAsync(GraphRelationship);
            return RedirectToPage("./Index");
        }

        private async Task LoadNodesAsync()
        {
            var nodes = await _service.GetAllNodesAsync();
            NodesList = new SelectList(nodes, "Id", "Label");
        }
    }
}
