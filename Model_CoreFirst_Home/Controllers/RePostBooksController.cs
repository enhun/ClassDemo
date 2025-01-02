using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

namespace Model_CoreFirst_Home.Controllers
{
    public class RePostBooksController : Controller
    {
        private readonly GuestBookContext _context;

        public RePostBooksController(GuestBookContext context)
        {
            _context = context;
        }

        


        // GET: RePostBooks
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReBook.ToListAsync());
        }

        // GET: RePostBooks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reBook = await _context.ReBook
                .FirstOrDefaultAsync(m => m.ReBookID == id);
            if (reBook == null)
            {
                return NotFound();
            }

            return View(reBook);
        }

        // GET: RePostBooks/Create
        public IActionResult Create(string bookId)
        {
            return PartialView(new ReBook { BookID = bookId });
        }

        // POST: RePostBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] ReBook reBook)
        {
            if (ModelState.IsValid)
            {
                reBook.ReBookID = Guid.NewGuid().ToString();
                reBook.TimeStamp = DateTime.Now;
                _context.Add(reBook);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = reBook.ReBookID,
                        author = reBook.Author,
                        description = reBook.Description,
                        timestamp = reBook.TimeStamp.ToString("yyyy/MM/dd HH:mm:ss")
                    }
                });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // GET: RePostBooks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reBook = await _context.ReBook.FindAsync(id);
            if (reBook == null)
            {
                return NotFound();
            }
            return View(reBook);
        }

        // POST: RePostBooks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReBookID,BookID,Author,Description,TimeStamp")] ReBook reBook)
        {
            if (id != reBook.ReBookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReBookExists(reBook.ReBookID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reBook);
        }

        // POST: RePostBooks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "無效的ID" });
                }

                var reply = await _context.ReBook.FindAsync(id);
                if (reply == null)
                {
                    return Json(new { success = false, message = "找不到留言" });
                }

                _context.ReBook.Remove(reply);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ReBookExists(string id)
        {
            return _context.ReBook.Any(e => e.ReBookID == id);
        }
    }
}