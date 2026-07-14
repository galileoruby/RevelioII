using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Nodes
{
    public class DeleteModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public DeleteModel(IGraphManagementService service)
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

        public async Task<IActionResult> OnPostAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return NotFound();

            await _service.DeleteNodeAsync(id.Value, cancellationToken);
            return RedirectToPage("./Index");
        }
    }
}
