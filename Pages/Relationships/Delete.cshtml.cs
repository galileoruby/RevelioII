using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RevelioII.Models;
using RevelioII.Services;

namespace RevelioII.Pages.Relationships
{
    public class DeleteModel : PageModel
    {
        private readonly IGraphManagementService _service;

        public DeleteModel(IGraphManagementService service)
        {
            _service = service;
        }

        [BindProperty]
        public Relationship GraphRelationship { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _service.DeleteRelationshipAsync(id.Value);
            return RedirectToPage("./Index");
        }
    }
}
