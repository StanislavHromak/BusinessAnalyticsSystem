using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        private bool CheckAccess(params string[] roles)
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role != null && roles.Contains(role);
        }

        // ==== CREATE ====
        [HttpGet]
        public IActionResult AddData()
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddData(FinancialData model)
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                model.CalculateKPI();
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }

        // ==== READ ====
        public async Task<IActionResult> List()
        {
            if (!CheckAccess("Admin", "Owner", "Investor"))
                return RedirectToAction("AccessDenied", "Home");

            var data = await _context.FinancialDatas
                .OrderByDescending(d => d.Date)
                .ToListAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!CheckAccess("Admin", "Owner", "Investor"))
                return RedirectToAction("AccessDenied", "Home");

            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ==== UPDATE ====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");

            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FinancialData model)
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                model.CalculateKPI();
                _context.FinancialDatas.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }

        // ==== DELETE ====
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");

            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CheckAccess("Admin", "Owner")) return RedirectToAction("AccessDenied", "Home");

            var item = await _context.FinancialDatas.FindAsync(id);
            if (item != null)
            {
                _context.FinancialDatas.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(List));
        }

    }
}

