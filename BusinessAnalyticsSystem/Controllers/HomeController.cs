using Microsoft.AspNetCore.Mvc;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessAnalyticsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // === Welcome Page ===
        public IActionResult Index()
        {
            return View();
        }

        // === Registration (default role = Investor) ===
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                var exists = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (exists != null)
                {
                    ViewBag.Error = "User already exists.";
                    return View(model);
                }

                model.Role = "Investor"; // default role
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Registration successful. Please login.";
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        // === Login ===
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // === Logout ===
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        // === Profile ===
        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction(nameof(Login));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return View(user);
        }

        // === Dashboard ===
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction(nameof(Login));

            return View();
        }

        // === Access Denied ===
        public IActionResult AccessDenied() => View();
    }
}


