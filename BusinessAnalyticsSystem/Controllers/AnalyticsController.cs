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

                // (3) Call calculation
                model.CalculateKPI();

                // (4) Save full model to database
                _context.FinancialDatas.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            // (5) Return ViewModel in case of error
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

            // Load related sales for this date if generated from sales
            if (item.IsGeneratedFromSales)
            {
                var relatedSales = await _context.Sales
                    .Include(s => s.Product)
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
            // 1. Find original model
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null) return NotFound();

            // 2. Create ViewModel and map data to it
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

            // 3. Pass ViewModel to View
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditDataViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 1. Find original record in database
                var model = await _context.FinancialDatas.FindAsync(viewModel.Id);
                if (model == null) return NotFound();

                // 2. Map updated data from ViewModel back to database model
                model.Date = viewModel.Date;
                model.FixedCosts = viewModel.FixedCosts;
                model.VariableCostPerUnit = viewModel.VariableCostPerUnit;
                model.PricePerUnit = viewModel.PricePerUnit;
                model.UnitsSold = viewModel.UnitsSold;
                model.Investment = viewModel.Investment;

                // 3. Recalculate KPI
                model.CalculateKPI();

                // 4. Update and save
                _context.FinancialDatas.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            // In case of validation error, return ViewModel
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

            // We will use "dynamic" to store the grouping result
            // since the keys (Key) will be different.
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
                        // (d.Date.Month - 1) / 3 + 1 -- this is the formula for calculating the quarter
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
                    period = "Month"; // Set default value
                    break;
            }

            // Recalculate KPI based on aggregated sums
            var finalReportData = (aggregatedReport as IEnumerable<dynamic>)
                .Select(m => new
                {
                    Label = m.Label,
                    m.Revenue,
                    m.TotalCosts,
                    m.Profit,
                    // Recalculate ROI and ROS based on monthly summaries
                    ROI = m.Investment > 0 ? (m.Profit / m.Investment) * 100 : 0,
                    ROS = m.Revenue > 0 ? (m.Profit / m.Revenue) * 100 : 0
                }).ToList();


            // Prepare data for charts
            var reportModel = new
            {
                Labels = finalReportData.Select(d => d.Label),
                Revenues = finalReportData.Select(d => d.Revenue),
                TotalCosts = finalReportData.Select(d => d.TotalCosts),
                Profits = finalReportData.Select(d => d.Profit),
                ROIs = finalReportData.Select(d => d.ROI),
                ROSs = finalReportData.Select(d => d.ROS)
            };

            // Pass current period to View to "highlight" active button
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
            // Default to last 30 days if not specified
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddDays(-30).Date;
            if (!endDate.HasValue)
                endDate = DateTime.Now.Date;

            // Aggregate sales by date
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

                // Calculate aggregated values
                var totalRevenue = (double)sales.Sum(s => s.TotalAmount);
                var totalUnitsSold = sales.Sum(s => s.Quantity);
                var avgUnitPrice = totalUnitsSold > 0 ? totalRevenue / totalUnitsSold : 0;

                // Estimate costs (you can adjust these based on your business logic)
                // For now, we'll use a simple estimation: variable cost = 60% of price, fixed cost = 10% of revenue
                var estimatedVariableCostPerUnit = avgUnitPrice * 0.6;
                var estimatedFixedCosts = totalRevenue * 0.1;
                var estimatedInvestment = 0.0; // Can be set manually later

                // Check if FinancialData already exists for this date
                var existingFinancialData = await _context.FinancialDatas
                    .FirstOrDefaultAsync(f => f.Date.Date == date);

                if (existingFinancialData != null)
                {
                    // Update existing record
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
                    // Create new record
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

