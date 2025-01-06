using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;


namespace MyModel_CodeFirst.Controllers
{
    public class BooksManageController : Controller
    {
        private readonly GuestBookContext _context;

        public BooksManageController(GuestBookContext context)
        {
            _context = context;
        }

        // GET: BooksManage
        public async Task<IActionResult> Index()
        {
            //4.2.1 改寫Index Action的內容，將留言依新到舊排序
            var books = await _context.Book.OrderByDescending(b => b.TimeStamp).ToListAsync();

            return View(books);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);

                //4.4.1 在BooksManageController中的Delete Action加入刪除圖片的程式
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BookPhotos", book.Photo ?? string.Empty);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //4.4.4 在BooksManageController加人中的DeleteReBook Action
        [HttpPost]
        public async Task<IActionResult> DeleteReBook(string id)
        {
            var reBook = await _context.ReBook.FindAsync(id);
            if (reBook != null)
            {
                _context.ReBook.Remove(reBook);

            }

            await _context.SaveChangesAsync();
            return Json(reBook);
        }


        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.BookID == id);
        }
    }
}
