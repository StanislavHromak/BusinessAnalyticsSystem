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
            var data = await _context.FinancialDatas
                .OrderBy(d => d.Date)
                .ToListAsync();

            if (data.Any())
            {
                // ������� ����� (��������� �� �����)
                var latest = data.Last();

                // ���� KPI �������� � ��� � ������������� ��, ������ ���������� �� ����
                double latestProfit = latest.Profit != 0 ? latest.Profit : latest.CalculateProfit();
                double latestProfitMargin = latest.ProfitMargin != 0 ? latest.ProfitMargin : latest.CalculateProfitMargin();
                double latestROI = latest.ROI != 0 ? latest.ROI : latest.CalculateROI();
                double latestBreakEven = latest.BreakEven != 0 ? latest.BreakEven : latest.CalculateBreakEven();

                ViewBag.Profit = latestProfit;
                ViewBag.ProfitMargin = latestProfitMargin;
                ViewBag.ROI = latestROI;
                ViewBag.BreakEven = latestBreakEven;

                // ������ �� ��� ������� (�� ��������� ��������)
                ViewBag.AvgProfit = data.Average(d => d.Profit != 0 ? d.Profit : d.CalculateProfit());
                ViewBag.AvgProfitMargin = data.Average(d => d.ProfitMargin != 0 ? d.ProfitMargin : d.CalculateProfitMargin());
                ViewBag.AvgROI = data.Average(d => d.ROI != 0 ? d.ROI : d.CalculateROI());

                // �������: ������� ������ (������� ������ �������)
                if (data.Count > 1)
                {
                    var profits = data.Select(d => d.Profit != 0 ? d.Profit : d.CalculateProfit()).ToList();
                    var avgGrowth = (profits.Last() - profits.First()) / (data.Count - 1);
                    ViewBag.ForecastedProfit = profits.Last() + avgGrowth;
                }
                else
                {
                    ViewBag.ForecastedProfit = null;
                }

                // Recommendation (�������� ������ ����� �� ����� ROI)
                ViewBag.Recommendation = latestROI > 10 ? "������������� ����������� ���." : "�������� ������� ��� ���������� ROI.";

                // ��� ��� ������� (JSON ��� Chart.js) � ������������� �������� ��������, ��� ��������
                ViewBag.ChartData = new
                {
                    Labels = data.Select(d => d.Date.ToShortDateString()).ToArray(),
                    Revenues = data.Select(d => d.Revenue).ToArray(),
                    Expenses = data.Select(d => d.Expenses).ToArray(),
                    Profits = data.Select(d => d.Profit != 0 ? d.Profit : d.CalculateProfit()).ToArray()
                };
            }

            return View();
        }
    }
}

