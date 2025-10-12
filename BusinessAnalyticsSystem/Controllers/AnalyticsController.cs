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
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // API ендпоінт
        [HttpGet("api/data")]
        public async Task<IActionResult> GetData()
        {
            var data = await _context.FinancialDatas.ToListAsync();
            return Ok(data);
        }
    }
}
