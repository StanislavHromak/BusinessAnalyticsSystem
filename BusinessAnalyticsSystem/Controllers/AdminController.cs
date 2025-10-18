using Microsoft.AspNetCore.Mvc;
using BusinessAnalyticsSystem.Data;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Check if current user is Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // === List of users ===
        public async Task<IActionResult> UserManagement()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // === Change user role ===
        [HttpPost]
        public async Task<IActionResult> ChangeRole(int id, string newRole)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = newRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserManagement));
        }

        // === Delete user ===
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(UserManagement));
        }
    }
}

