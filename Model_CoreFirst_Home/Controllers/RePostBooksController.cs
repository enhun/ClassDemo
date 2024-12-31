using Microsoft.AspNetCore.Mvc;
using Model_CoreFirst_Home.Models;

public class RePostBooksController : Controller
{
    private readonly GuestBookContext _context;

    public RePostBooksController(GuestBookContext context)
    {
        _context = context;
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
}