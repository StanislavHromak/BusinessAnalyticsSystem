using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers
{
    [Authorize] 
    public class AnalyticsController : Controller
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // ==== CREATE ====
        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public IActionResult AddData()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> AddData(FinancialData model)
        {
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
        [Authorize(Roles = "Admin,Owner,Investor")]
        public async Task<IActionResult> List()
        {
            var data = await _context.FinancialDatas
                .OrderByDescending(d => d.Date)
                .ToListAsync();
            return View(data);
        }

        [Authorize(Roles = "Admin,Owner,Investor")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ==== UPDATE ====
        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> Edit(FinancialData model)
        {
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
        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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

