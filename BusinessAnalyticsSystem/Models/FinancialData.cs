using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    public class FinancialData
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public double Revenue { get; set; }      // Доходи
        public double Expenses { get; set; }     // Витрати
        public double Investment { get; set; }   // Інвестиції
        public double FixedCosts { get; set; }   // Фіксовані витрати
        public double VariableCostsPerUnit { get; set; } // Змінні витрати
        public double PricePerUnit { get; set; } // Ціна за одиницю
        public int UnitsSold { get; set; }       // Продані одиниці

        // KPI — зберігатимуться в базі
        public double Profit { get; set; }
        public double ProfitMargin { get; set; }
        public double ROI { get; set; }
        public double BreakEven { get; set; }

        // Методи обчислення
        public double CalculateProfit() => Revenue - Expenses;
        public double CalculateProfitMargin() => Revenue > 0 ? (CalculateProfit() / Revenue) * 100 : 0;
        public double CalculateROI() => Investment > 0 ? (CalculateProfit() / Investment) * 100 : 0;
        public double CalculateBreakEven() => (PricePerUnit - VariableCostsPerUnit) > 0
            ? FixedCosts / (PricePerUnit - VariableCostsPerUnit)
            : 0;

        public void CalculateAllKPI()
        {
            Profit = CalculateProfit();
            ProfitMargin = CalculateProfitMargin();
            ROI = CalculateROI();
            BreakEven = CalculateBreakEven();
        }
    }
}
