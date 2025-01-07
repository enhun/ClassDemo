using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

public class LoginController : Controller
{
    private readonly GuestBookContext _context;
    public LoginController(GuestBookContext context)
    {
        _context = context;
    }

    // GET: Login
    public IActionResult Index()
    {
        return View("Login"); // 確保有 Login.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login login)
    {
        if (login == null || string.IsNullOrEmpty(login.Account) || string.IsNullOrEmpty(login.Password))
        {
            ViewData["Message"] = "請輸入帳號和密碼";
            return View("Login", login);
        }

        var result = await _context.Login
            .Where(m => m.Account == login.Account && m.Password == login.Password)
            .FirstOrDefaultAsync();

        if (result != null)
        {
            HttpContext.Session.SetString("Manager", result.Account); // 改成與 Layout 一致的 key
            return RedirectToAction("Index", "BooksManage");
        }

        ViewData["Message"] = "帳號或密碼錯誤";
        return View("Login", login); // 返回原始的 login model，而不是 result
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Manager");
        return RedirectToAction("Index", "Home");
    }

}