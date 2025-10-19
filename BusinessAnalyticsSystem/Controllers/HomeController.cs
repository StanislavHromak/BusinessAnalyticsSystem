using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index() => View();

        // === Registration (default role = Investor) ===
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email || u.Phone == model.Phone);

                if (existingUser != null)
                {
                    ViewBag.Error = "User with this username, email, or phone already exists.";
                    return View(model);
                }

                model.Role = "Investor";
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Registration successful. Please log in.";
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

                return RedirectToAction(nameof(Dashboard));
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
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
                return RedirectToAction(nameof(Login));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
                return RedirectToAction(nameof(Login));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                user.Username = model.Username;
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;

                // Якщо користувач ввів новий пароль — оновлюємо
                if (!string.IsNullOrWhiteSpace(model.Password))
                    user.Password = model.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Оновимо сесію (щоб збереглося нове ім’я, якщо воно змінилося)
                HttpContext.Session.SetString("Username", user.Username);

                ViewBag.Message = "Profile updated successfully!";
            }

            return View(user);
        }


        // === Dashboard ===
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction(nameof(Login));

            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}



