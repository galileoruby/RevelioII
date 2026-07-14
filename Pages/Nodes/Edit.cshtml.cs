using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Nodes
{
    public class EditModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public EditModel(IGraphManagementService service)
        {
            _service = service;
        }

        [BindProperty]
        public Node GraphNode { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return NotFound();

            var node = await _service.GetNodeAsync(id.Value, cancellationToken);
            if (node == null) return NotFound();

            GraphNode = node;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return Page();

            await _service.UpdateNodeAsync(GraphNode, cancellationToken);
            return RedirectToPage("./Index");
        }
    }
}
