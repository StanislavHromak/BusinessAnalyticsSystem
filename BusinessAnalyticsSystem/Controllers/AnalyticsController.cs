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
            // Повертаємо нову ViewModel
            return View(new AddDataViewModel());
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> AddData(AddDataViewModel viewModel) // (1) Приймаємо ViewModel
        {
            if (ModelState.IsValid)
            {
                // (2) Створюємо повну модель даних
                var model = new FinancialData
                {
                    Date = viewModel.Date,
                    FixedCosts = viewModel.FixedCosts,
                    VariableCostPerUnit = viewModel.VariableCostPerUnit,
                    PricePerUnit = viewModel.PricePerUnit,
                    UnitsSold = viewModel.UnitsSold,
                    Investment = viewModel.Investment
                };

                // (3) Викликаємо розрахунок
                model.CalculateKPI();

                // (4) Зберігаємо повну модель в БД
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            // (5) Повертаємо ViewModel у разі помилки
            return View(viewModel);
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
            // 1. Знаходимо оригінальну модель
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();

            // 2. Створюємо ViewModel і перекладаємо в неї дані
            var viewModel = new EditDataViewModel
            {
                Id = item.Id,
                Date = item.Date,
                FixedCosts = item.FixedCosts,
                VariableCostPerUnit = item.VariableCostPerUnit,
                PricePerUnit = item.PricePerUnit,
                UnitsSold = item.UnitsSold,
                Investment = item.Investment
            };

            // 3. Передаємо ViewModel у View
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditDataViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 1. Знаходимо оригінальний запис у БД
                var model = await _context.FinancialDatas.FindAsync(viewModel.Id);
                if (model == null) return NotFound();

                // 2. Перекладаємо оновлені дані з ViewModel назад у модель БД
                model.Date = viewModel.Date;
                model.FixedCosts = viewModel.FixedCosts;
                model.VariableCostPerUnit = viewModel.VariableCostPerUnit;
                model.PricePerUnit = viewModel.PricePerUnit;
                model.UnitsSold = viewModel.UnitsSold;
                model.Investment = viewModel.Investment;

                // 3. Перераховуємо KPI
                model.CalculateKPI();

                // 4. Оновлюємо та зберігаємо
                _context.FinancialDatas.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            // У разі помилки валідації повертаємо ViewModel
            return View(viewModel);
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

        // ===== НОВА ДІЯ ДЛЯ АНАЛІТИЧНОГО ЗВІТУ =====
        [Authorize(Roles = "Admin,Owner,Investor")]
        [HttpGet]
        public async Task<IActionResult> AnalysisReport(string period = "Month")
        {
            var dailyData = await _context.FinancialDatas.ToListAsync();

            // Ми будемо використовувати "dynamic", щоб зберігати результат 
            // групування, оскільки ключі (Key) будуть різними.
            dynamic aggregatedReport;

            switch (period)
            {
                case "Year":
                    aggregatedReport = dailyData
                        .GroupBy(d => new { d.Date.Year })
                        .OrderBy(g => g.Key.Year)
                        .Select(g => new
                        {
                            Label = $"{g.Key.Year}",
                            Revenue = g.Sum(x => x.Revenue),
                            TotalCosts = g.Sum(x => x.TotalCosts),
                            Profit = g.Sum(x => x.Profit),
                            Investment = g.Sum(x => x.Investment),
                        }).ToList();
                    break;

                case "Quarter":
                    aggregatedReport = dailyData
                        // (d.Date.Month - 1) / 3 + 1 -- це формула для розрахунку кварталу
                        .GroupBy(d => new { d.Date.Year, Quarter = (d.Date.Month - 1) / 3 + 1 })
                        .OrderBy(g => g.Key.Year)
                        .ThenBy(g => g.Key.Quarter)
                        .Select(g => new
                        {
                            Label = $"Q{g.Key.Quarter}/{g.Key.Year}",
                            Revenue = g.Sum(x => x.Revenue),
                            TotalCosts = g.Sum(x => x.TotalCosts),
                            Profit = g.Sum(x => x.Profit),
                            Investment = g.Sum(x => x.Investment),
                        }).ToList();
                    break;

                case "Month":
                default:
                    aggregatedReport = dailyData
                        .GroupBy(d => new { d.Date.Year, d.Date.Month })
                        .OrderBy(g => g.Key.Year)
                        .ThenBy(g => g.Key.Month)
                        .Select(g => new
                        {
                            Label = $"{g.Key.Month}/{g.Key.Year}",
                            Revenue = g.Sum(x => x.Revenue),
                            TotalCosts = g.Sum(x => x.TotalCosts),
                            Profit = g.Sum(x => x.Profit),
                            Investment = g.Sum(x => x.Investment),
                        }).ToList();
                    period = "Month"; // Встановлюємо значення за замовчуванням
                    break;
            }

            // Перераховуємо KPI на основі згрупованих сум
            var finalReportData = (aggregatedReport as IEnumerable<dynamic>)
                .Select(m => new
                {
                    Label = m.Label,
                    m.Revenue,
                    m.TotalCosts,
                    m.Profit,
                    // Перераховуємо ROI та ROS на основі місячних підсумків
                    ROI = m.Investment > 0 ? (m.Profit / m.Investment) * 100 : 0,
                    ROS = m.Revenue > 0 ? (m.Profit / m.Revenue) * 100 : 0
                }).ToList();


            // Готуємо дані для графіків
            var reportModel = new
            {
                Labels = finalReportData.Select(d => d.Label),
                Revenues = finalReportData.Select(d => d.Revenue),
                TotalCosts = finalReportData.Select(d => d.TotalCosts),
                Profits = finalReportData.Select(d => d.Profit),
                ROIs = finalReportData.Select(d => d.ROI),
                ROSs = finalReportData.Select(d => d.ROS)
            };

            // Передаємо поточний період у View, щоб "підсвітити" активну кнопку
            ViewData["CurrentPeriod"] = period;

            return View(reportModel);
        }
    }
}

