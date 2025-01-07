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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            // select * from Login where Account = @Account and Password = @Password
            var result = await _context.Login.Where(m => m.Account == login.Account && m.Password == login.Password).FirstOrDefaultAsync();


            if (result != null)
            {
                return RedirectToAction("Index", "BooksManage");
            }
            else
            {
                ViewData["Message"] = "帳號或密碼錯誤";
            }

            return View(result);
        }
    }
}
