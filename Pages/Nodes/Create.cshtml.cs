using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Nodes
{
    public class CreateModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public CreateModel(IGraphManagementService service)
        {
            _service = service;
        }

        [BindProperty]
        public Node GraphNode { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.CreateNodeAsync(GraphNode, cancellationToken);
            return RedirectToPage("./Index");
        }
    }
}
