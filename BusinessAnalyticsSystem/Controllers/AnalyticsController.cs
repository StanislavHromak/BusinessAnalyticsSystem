using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAnalyticsSystem.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // ==== CREATE ====
        [HttpGet]
        public IActionResult AddData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddData(FinancialData model)
        {
            if (ModelState.IsValid)
            {
                model.CalculateAllKPI(); // автообчислення KPI
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }

        // ==== READ ====
        public async Task<IActionResult> List()
        {
            var data = await _context.FinancialDatas
                .OrderByDescending(d => d.Date)
                .ToListAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ==== UPDATE ====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FinancialData model)
        {
            if (ModelState.IsValid)
            {
                model.CalculateAllKPI();
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
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
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

        // ==== API ====
        [HttpGet("api/data")]
        public async Task<IActionResult> GetData()
        {
            var data = await _context.FinancialDatas.ToListAsync();
            return Ok(data);
        }
    }
}
