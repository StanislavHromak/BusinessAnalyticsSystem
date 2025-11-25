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
            // Return new ViewModel
            return View(new AddDataViewModel());
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> AddData(AddDataViewModel viewModel) // (1) Accept ViewModel
        {
            if (ModelState.IsValid)
            {
                // (2) Create full data model
                var model = new FinancialData
                {
                    Date = viewModel.Date,
                    FixedCosts = viewModel.FixedCosts,
                    VariableCostPerUnit = viewModel.VariableCostPerUnit,
                    PricePerUnit = viewModel.PricePerUnit,
                    UnitsSold = viewModel.UnitsSold,
                    Investment = viewModel.Investment
                };

                model.CalculateKPI();
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

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

            if (item.IsGeneratedFromSales)
            {
                var relatedSales = await _context.Sales
                    .Include(s => s.Product)
                        .ThenInclude(p => p.Category)
                    .Include(s => s.Department)
                    .Where(s => s.SaleDateTime.Date == item.Date.Date)
                    .ToListAsync();
                item.RelatedSales = relatedSales;
            }

            return View(item);
        }

        // ==== UPDATE ====
        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();

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

            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditDataViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = await _context.FinancialDatas.FindAsync(viewModel.Id);
                if (model == null) return NotFound();

                model.Date = viewModel.Date;
                model.FixedCosts = viewModel.FixedCosts;
                model.VariableCostPerUnit = viewModel.VariableCostPerUnit;
                model.PricePerUnit = viewModel.PricePerUnit;
                model.UnitsSold = viewModel.UnitsSold;
                model.Investment = viewModel.Investment;

                model.CalculateKPI();

                _context.FinancialDatas.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

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

        // ===== NEW ACTION FOR ANALYTICAL REPORT =====
        [Authorize(Roles = "Admin,Owner,Investor")]
        [HttpGet]
        public async Task<IActionResult> AnalysisReport(string period = "Month")
        {
            var dailyData = await _context.FinancialDatas.ToListAsync();
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
                    period = "Month"; 
                    break;
            }

            var finalReportData = (aggregatedReport as IEnumerable<dynamic>)
                .Select(m => new
                {
                    Label = m.Label,
                    m.Revenue,
                    m.TotalCosts,
                    m.Profit,
                    ROI = m.Investment > 0 ? (m.Profit / m.Investment) * 100 : 0,
                    ROS = m.Revenue > 0 ? (m.Profit / m.Revenue) * 100 : 0
                }).ToList();


            var reportModel = new
            {
                Labels = finalReportData.Select(d => d.Label),
                Revenues = finalReportData.Select(d => d.Revenue),
                TotalCosts = finalReportData.Select(d => d.TotalCosts),
                Profits = finalReportData.Select(d => d.Profit),
                ROIs = finalReportData.Select(d => d.ROI),
                ROSs = finalReportData.Select(d => d.ROS)
            };

            ViewData["CurrentPeriod"] = period;

            return View(reportModel);
        }

        // ===== GENERATE FINANCIAL DATA FROM SALES =====
        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public IActionResult GenerateFromSales()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateFromSales(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddDays(-30).Date;
            if (!endDate.HasValue)
                endDate = DateTime.Now.Date;

            var salesByDate = await _context.Sales
                .Include(s => s.Product)
                .Where(s => s.SaleDateTime.Date >= startDate.Value.Date && s.SaleDateTime.Date <= endDate.Value.Date)
                .GroupBy(s => s.SaleDateTime.Date)
                .ToListAsync();

            int createdCount = 0;
            int updatedCount = 0;

            foreach (var group in salesByDate)
            {
                var date = group.Key;
                var sales = group.ToList();

                var totalRevenue = (double)sales.Sum(s => s.TotalAmount);
                var totalUnitsSold = sales.Sum(s => s.Quantity);
                var avgUnitPrice = totalUnitsSold > 0 ? totalRevenue / totalUnitsSold : 0;
              
                var estimatedVariableCostPerUnit = avgUnitPrice * 0.6;
                var estimatedFixedCosts = totalRevenue * 0.1;
                var estimatedInvestment = 0.0; 

                var existingFinancialData = await _context.FinancialDatas
                    .FirstOrDefaultAsync(f => f.Date.Date == date);

                if (existingFinancialData != null)
                {
                    existingFinancialData.PricePerUnit = avgUnitPrice;
                    existingFinancialData.UnitsSold = totalUnitsSold;
                    existingFinancialData.VariableCostPerUnit = estimatedVariableCostPerUnit;
                    existingFinancialData.FixedCosts = estimatedFixedCosts;
                    existingFinancialData.Investment = estimatedInvestment;
                    existingFinancialData.IsGeneratedFromSales = true;
                    existingFinancialData.CalculateKPI();
                    updatedCount++;
                }
                else
                {
                    var financialData = new FinancialData
                    {
                        Date = date,
                        PricePerUnit = avgUnitPrice,
                        UnitsSold = totalUnitsSold,
                        VariableCostPerUnit = estimatedVariableCostPerUnit,
                        FixedCosts = estimatedFixedCosts,
                        Investment = estimatedInvestment,
                        IsGeneratedFromSales = true
                    };
                    financialData.CalculateKPI();
                    _context.FinancialDatas.Add(financialData);
                    createdCount++;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully generated Financial Data from Sales: {createdCount} created, {updatedCount} updated.";
            return RedirectToAction(nameof(List));
        }
    }
}

