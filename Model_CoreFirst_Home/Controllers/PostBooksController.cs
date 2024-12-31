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
                      .Include(b => b.ReBooks)  // 加載關聯的留言
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
                    .FirstOrDefaultAsync(b => b.BookID == id);

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
                        if (!ValidateBookImageFile(book.ImageFile))
                        {
                            return View(book);
                        }

                        book.Photo = await SaveBookImageFile(book.ImageFile);
                    }

                    book.TimeStamp = DateTime.Now;
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "貼文新增成功！";
                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating book");
                ModelState.AddModelError("", "新增貼文時發生錯誤，請稍後再試。");
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
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == id);

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
                        if (!ValidateBookImageFile(book.ImageFile))
                        {
                            return View(book);
                        }

                        await DeleteImageFile(book.Photo);
                        book.Photo = await SaveBookImageFile(book.ImageFile);
                    }

                    book.TimeStamp = DateTime.Now;
                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "貼文已成功更新！";
                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating book with ID {id}");
                ModelState.AddModelError("", "更新貼文時發生錯誤，請稍後再試。");
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
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == id);

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
                var book = await _context.Book.Include(b => b.ReBooks).FirstOrDefaultAsync(b => b.BookID == id);

                if (book != null)
                {
                    if (book.ReBooks != null && book.ReBooks.Any())
                    {
                        _context.ReBook.RemoveRange(book.ReBooks);
                    }

                    if (!string.IsNullOrEmpty(book.Photo))
                    {
                        await DeleteImageFile(book.Photo);
                    }

                    _context.Book.Remove(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "貼文已成功刪除";
                }
                else
                {
                    TempData["Error"] = "找不到該貼文";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting book with ID {id}");
                TempData["Error"] = "刪除貼文時發生錯誤，請稍後再試。";
                return RedirectToAction(nameof(Index));
            }
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

        private async Task<string> SaveBookImageFile(IFormFile file)
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

        private bool ValidateBookImageFile(IFormFile file)
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
    }
}
