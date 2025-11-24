using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Controllers.Api
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/analytics/data
        [HttpGet("data")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAllData()
        {
            var data = await _context.FinancialDatas
                .OrderByDescending(d => d.Date)
                .Select(d => new
                {
                    d.Id,
                    Date = d.Date.ToString("yyyy-MM-dd"),
                    d.FixedCosts,
                    d.VariableCostPerUnit,
                    d.PricePerUnit,
                    d.UnitsSold,
                    d.Investment,
                    d.Revenue,
                    d.TotalCosts,
                    d.Profit,
                    d.ROI,
                    d.ROS
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/analytics/data/5
        [HttpGet("data/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetData(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                item.Id,
                Date = item.Date.ToString("yyyy-MM-dd"),
                item.FixedCosts,
                item.VariableCostPerUnit,
                item.PricePerUnit,
                item.UnitsSold,
                item.Investment,
                item.Revenue,
                item.TotalCosts,
                item.Profit,
                item.ROI,
                item.ROS
            });
        }

        // POST: api/analytics/data
        [HttpPost("data")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CreateData([FromBody] FinancialDataDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new FinancialData
            {
                Date = dto.Date,
                FixedCosts = dto.FixedCosts,
                VariableCostPerUnit = dto.VariableCostPerUnit,
                PricePerUnit = dto.PricePerUnit,
                UnitsSold = dto.UnitsSold,
                Investment = dto.Investment
            };

            model.CalculateKPI();

            _context.FinancialDatas.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetData), new { id = model.Id }, new
            {
                model.Id,
                Date = model.Date.ToString("yyyy-MM-dd"),
                model.FixedCosts,
                model.VariableCostPerUnit,
                model.PricePerUnit,
                model.UnitsSold,
                model.Investment,
                model.Revenue,
                model.TotalCosts,
                model.Profit,
                model.ROI,
                model.ROS
            });
        }

        // PUT: api/analytics/data/5
        [HttpPut("data/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateData(int id, [FromBody] FinancialDataDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _context.FinancialDatas.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            model.Date = dto.Date;
            model.FixedCosts = dto.FixedCosts;
            model.VariableCostPerUnit = dto.VariableCostPerUnit;
            model.PricePerUnit = dto.PricePerUnit;
            model.UnitsSold = dto.UnitsSold;
            model.Investment = dto.Investment;

            model.CalculateKPI();

            _context.FinancialDatas.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/analytics/data/5
        [HttpDelete("data/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteData(int id)
        {
            var item = await _context.FinancialDatas.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.FinancialDatas.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/analytics/report?period=Month
        [HttpGet("report")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAnalysisReport([FromQuery] string period = "Month")
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
                labels = finalReportData.Select(d => d.Label).ToArray(),
                revenues = finalReportData.Select(d => (double)d.Revenue).ToArray(),
                totalCosts = finalReportData.Select(d => (double)d.TotalCosts).ToArray(),
                profits = finalReportData.Select(d => (double)d.Profit).ToArray(),
                rois = finalReportData.Select(d => (double)d.ROI).ToArray(),
                ross = finalReportData.Select(d => (double)d.ROS).ToArray()
            };

            return Ok(reportModel);
        }

        // GET: api/analytics/dashboard
        [HttpGet("dashboard")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var allData = await _context.FinancialDatas.ToListAsync();

            var stats = new
            {
                TotalRevenue = allData.Sum(d => d.Revenue),
                TotalExpenses = allData.Sum(d => d.TotalCosts),
                Profit = allData.Sum(d => d.Profit),
                TotalRecords = allData.Count
            };

            return Ok(stats);
        }
    }

    public class FinancialDataDto
    {
        public DateTime Date { get; set; }
        public double FixedCosts { get; set; }
        public double VariableCostPerUnit { get; set; }
        public double PricePerUnit { get; set; }
        public int UnitsSold { get; set; }
        public double Investment { get; set; }
    }
}

