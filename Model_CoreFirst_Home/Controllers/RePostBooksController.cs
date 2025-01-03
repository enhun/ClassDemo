using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model_CoreFirst_Home.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Model_CoreFirst_Home.Controllers
{
    public class RePostBooksController : Controller
    {
        private readonly GuestBookContext _context;
        private readonly ILogger<RePostBooksController> _logger;

        public RePostBooksController(GuestBookContext context, ILogger<RePostBooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: RePostBooks
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.ReBook.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ReBooks");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: RePostBooks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var reBook = await _context.ReBook
                    .FirstOrDefaultAsync(m => m.ReBookID == id);

                if (reBook == null)
                {
                    return NotFound($"ReBook with ID {id} not found");
                }

                return View(reBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching ReBook with ID {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: RePostBooks/Create
        public IActionResult Create(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                return BadRequest("Invalid BookID");
            }
            return PartialView(new ReBook { BookID = bookId });
        }

        // POST: RePostBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ReBook reBook)
        {
            try
            {
                if (string.IsNullOrEmpty(reBook.Author))
                {
                    return Json(new { success = false, message = "請輸入回覆者名稱" });
                }

                if (string.IsNullOrEmpty(reBook.Description))
                {
                    return Json(new { success = false, message = "請輸入回覆內容" });
                }

                if (string.IsNullOrEmpty(reBook.BookID))
                {
                    return Json(new { success = false, message = "找不到對應的貼文" });
                }

                var book = await _context.Book.FindAsync(reBook.BookID);
                if (book == null)
                {
                    return Json(new { success = false, message = "找不到對應的貼文" });
                }

                // 設定必要欄位
                if (string.IsNullOrEmpty(reBook.ReBookID))
                {
                    reBook.ReBookID = Guid.NewGuid().ToString();
                }
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ReBook: {@ReBook}", reBook);
                return Json(new { success = false, message = "發生錯誤，請稍後再試" });
            }
        }

        // GET: RePostBooks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var reBook = await _context.ReBook.FindAsync(id);
                if (reBook == null)
                {
                    return NotFound($"ReBook with ID {id} not found");
                }
                return View(reBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching ReBook with ID {id} for edit");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: RePostBooks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReBookID,BookID,Author,Description,TimeStamp")] ReBook reBook)
        {
            if (id != reBook.ReBookID)
            {
                return BadRequest("ID mismatch");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reBook.TimeStamp = DateTime.Now;
                    _context.Update(reBook);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ReBookExists(reBook.ReBookID))
                    {
                        return NotFound($"ReBook with ID {id} not found");
                    }
                    _logger.LogError(ex, $"Concurrency error occurred while updating ReBook with ID {id}");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while updating ReBook with ID {id}");
                    return StatusCode(500, "An error occurred while processing your request.");
                }
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

                return Json(new { success = true, message = "留言已成功刪除" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting ReBook with ID {id}");
                return Json(new { success = false, message = "刪除留言時發生錯誤，請稍後再試" });
            }
        }

        private bool ReBookExists(string id)
        {
            return _context.ReBook.Any(e => e.ReBookID == id);
        }
    }
}