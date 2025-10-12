using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BusinessAnalyticsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.FinancialDatas.ToListAsync();
            if (data.Any())
            {
                var latest = data.Last();
                ViewBag.Profit = latest.CalculateProfit();
                ViewBag.ProfitMargin = latest.CalculateProfitMargin();
                ViewBag.ROI = latest.CalculateROI();
                ViewBag.BreakEven = latest.CalculateBreakEven();

                // Прогноз: Простий лінійний (середній приріст профіту)
                if (data.Count > 1)
                {
                    var profits = data.Select(d => d.CalculateProfit()).ToList();
                    var avgGrowth = (profits.Last() - profits.First()) / (data.Count - 1);
                    ViewBag.ForecastedProfit = profits.Last() + avgGrowth;
                }

                // Рекомендації
                ViewBag.Recommendation = latest.CalculateROI() > 10 ? "Рекомендується інвестувати далі." : "Зменшіть витрати для покращення ROI.";

                // Дані для графіка (JSON для Chart.js)
                ViewBag.ChartData = new
                {
                    Labels = data.Select(d => d.Date.ToShortDateString()).ToArray(),
                    Revenues = data.Select(d => d.Revenue).ToArray(),
                    Expenses = data.Select(d => d.Expenses).ToArray()
                };
            }
            return View();
        }
    }
}
