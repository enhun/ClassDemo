using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;


namespace MyModel_CodeFirst.ViewComponents
{
    //3.2.5  撰寫VCBooksTopThree Class使其能讀出最新三筆留言
    public class VCBooksTopThree : ViewComponent
    {
        private readonly GuestBookContext _context;

        public VCBooksTopThree(GuestBookContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var books = await _context.Book.OrderByDescending(b => b.TimeStamp).Take(3).ToListAsync();
            return View(books);
        }
    }
}

