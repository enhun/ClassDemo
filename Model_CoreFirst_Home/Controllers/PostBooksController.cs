using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Model_CoreFirst_Home.Controllers
{
    public class PostBooksController : Controller
    {
        private readonly GuestBookContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<PostBooksController> _logger;
        private const int PageSize = 10;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public PostBooksController(
            GuestBookContext context,
            IWebHostEnvironment hostEnvironment,
            ILogger<PostBooksController> logger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        // GET: PostBooks
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var totalItems = await _context.Book.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                page = Math.Max(1, Math.Min(page, totalPages));

                var books = await _context.Book
                    .OrderByDescending(b => b.TimeStamp)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: PostBooks/Display/5
        public async Task<IActionResult> Display(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var book = await _context.Book
                    .Include(b => b.ReBooks)
                    .FirstOrDefaultAsync(m => m.BookID == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching book with ID {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // 在這裡加入新的 AddReply action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(string BookID, string Author, string Description)
        {
            if (string.IsNullOrEmpty(BookID) || string.IsNullOrEmpty(Author) || string.IsNullOrEmpty(Description))
            {
                TempData["Error"] = "請填寫所有必要欄位";
                return RedirectToAction(nameof(Display), new { id = BookID });
            }

            try
            {
                var reply = new ReBook
                {
                    ReBookID = Guid.NewGuid().ToString(),
                    BookID = BookID,
                    Author = Author.Trim(),
                    Description = Description.Trim(),
                    TimeStamp = DateTime.Now
                };

                _context.ReBook.Add(reply);
                await _context.SaveChangesAsync();

                TempData["Success"] = "留言發表成功！";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "留言發表失敗，請稍後再試";
            }

            return RedirectToAction(nameof(Display), new { id = BookID });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(string replyId, string bookId)
        {
            if (string.IsNullOrEmpty(replyId))
            {
                TempData["Error"] = "無效的留言ID";
                return RedirectToAction(nameof(Display), new { id = bookId });
            }

            try
            {
                var reply = await _context.ReBook.FindAsync(replyId);
                if (reply != null)
                {
                    _context.ReBook.Remove(reply);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "留言已成功刪除";
                }
                else
                {
                    TempData["Error"] = "找不到該留言";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除留言時發生錯誤");
                TempData["Error"] = "刪除留言失敗，請稍後再試";
            }

            return RedirectToAction(nameof(Display), new { id = bookId });
        }



        // GET: PostBooks/Create
        public IActionResult Create()
        {
            return View(new Book { BookID = Guid.NewGuid().ToString() });
        }

        // POST: PostBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookID,Title,Description,Author,ImageFile,TimeStamp")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (book.ImageFile != null)
                    {
                        if (!ValidateImageFile(book.ImageFile))
                        {
                            return View(book);
                        }

                        book.Photo = await SaveImageFile(book.ImageFile);
                    }

                    book.TimeStamp = DateTime.Now;
                    _context.Add(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully created book with ID {book.BookID}");
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating book");
                ModelState.AddModelError("", "An error occurred while saving the book. Please try again.");
                return View(book);
            }
        }

        // GET: PostBooks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var book = await _context.Book.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching book with ID {id} for edit");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: PostBooks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BookID,Title,Description,Author,ImageFile,Photo,TimeStamp")] Book book)
        {
            if (id != book.BookID)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (book.ImageFile != null)
                    {
                        if (!ValidateImageFile(book.ImageFile))
                        {
                            return View(book);
                        }

                        // Delete old image
                        await DeleteImageFile(book.Photo);

                        // Save new image
                        book.Photo = await SaveImageFile(book.ImageFile);
                    }

                    book.TimeStamp = DateTime.Now;
                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully updated book with ID {book.BookID}");
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookExists(book.BookID))
                {
                    return NotFound($"Book with ID {id} not found");
                }
                else
                {
                    _logger.LogError(ex, $"Concurrency error occurred while updating book with ID {id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating book with ID {id}");
                ModelState.AddModelError("", "An error occurred while saving changes. Please try again.");
                return View(book);
            }
        }

        // GET: PostBooks/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }

            try
            {
                var book = await _context.Book
                    .FirstOrDefaultAsync(m => m.BookID == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching book with ID {id} for deletion");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }




        // POST: PostBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var book = await _context.Book.FindAsync(id);
                if (book != null)
                {
                    await DeleteImageFile(book.Photo);
                    _context.Book.Remove(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully deleted book with ID {id}");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting book with ID {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.BookID == id);
        }

        private bool ValidateImageFile(IFormFile file)
        {
            if (file.Length > MaxFileSize)
            {
                ModelState.AddModelError("ImageFile", "File size cannot exceed 5MB");
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg and .png files are allowed");
                return false;
            }

            return true;
        }

        private async Task<string> SaveImageFile(IFormFile file)
        {
            var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "BookPhotos");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        private async Task DeleteImageFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "BookPhotos", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                        await Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error deleting file: {fileName}");
                    }
                }
            }
        }
    }
}