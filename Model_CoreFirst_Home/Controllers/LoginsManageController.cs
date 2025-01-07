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
    public class LoginsManageController : Controller
    {
        private readonly GuestBookContext _context;

        public LoginsManageController(GuestBookContext context)
        {
            _context = context;
        }

        // GET: LoginsManage
        public async Task<IActionResult> Index()
        {
            return View(await _context.Login.ToListAsync());
        }



        // GET: LoginsManage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoginsManage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Account,Password")] Login login)
        {
            if (_context.Login.Find(login.Account) != null)
            {
                ViewData["Message"] = "帳號已存在";
                return View(login);
            }

            if (ModelState.IsValid)
            {
                _context.Add(login);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(login);
        }



        // GET: LoginsManage/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Login
                .FirstOrDefaultAsync(m => m.Account == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // POST: LoginsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var login = await _context.Login.FindAsync(id);
            if (login != null)
            {
                _context.Login.Remove(login);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginExists(string id)
        {
            return _context.Login.Any(e => e.Account == id);
        }
    }
}
