using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

namespace MyModel_CodeFirst.ViewComponents
{
    public class VCRebooksViewComponent : ViewComponent
    {
        private readonly GuestBookContext _context;

        public VCRebooksViewComponent(GuestBookContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string bookId, bool isDel = false)
        {
            var reBooks = await _context.ReBook
                .Where(r => r.BookID == bookId)
                .OrderByDescending(r => r.TimeStamp)
                .ToListAsync();
            return View(reBooks);
        }
    }
}
