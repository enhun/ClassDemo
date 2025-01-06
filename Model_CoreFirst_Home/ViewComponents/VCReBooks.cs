using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

namespace MyModel_CodeFirst.ViewComponents
{
    public class VCRebooks : ViewComponent
    {
        private readonly GuestBookContext _context;

        // 建構子，注入資料庫內容
        public VCRebooks(GuestBookContext context)
        {
            _context = context;
        }

        // 這個方法會被自動調用來取得資料
        public async Task<IViewComponentResult> InvokeAsync(string bookId)
        {
            var reBooks = await _context.ReBook
                .Where(r => r.BookID == bookId)
                .OrderByDescending(r => r.TimeStamp)
                .ToListAsync();

            return View(reBooks);  // 預設會找 Default.cshtml
        }
    }
}
