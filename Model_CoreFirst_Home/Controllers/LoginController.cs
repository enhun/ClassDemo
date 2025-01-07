using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

namespace Model_CoreFirst_Home.Controllers
{
    public class LoginController : Controller
    {
        private readonly GuestBookContext _context;

        public LoginController(GuestBookContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Index()  // 改成 Index
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (login == null || string.IsNullOrEmpty(login.Account) || string.IsNullOrEmpty(login.Password))
            {
                ViewData["Message"] = "請輸入帳號和密碼";
                return View(login);
            }

            var result = await _context.Login
                .Where(m => m.Account == login.Account && m.Password == login.Password)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                return RedirectToAction("Index", "BooksManage");
            }

            ViewData["Message"] = "帳號或密碼錯誤";
            return View(login);
        }
    }
}