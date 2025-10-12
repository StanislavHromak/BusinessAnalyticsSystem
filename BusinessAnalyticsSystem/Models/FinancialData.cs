namespace BusinessAnalyticsSystem.Models
{
    public class FinancialData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public double Revenue { get; set; }  // Доходи
        public double Expenses { get; set; } // Витрати
        public double Investment { get; set; } // Інвестиції
        public double FixedCosts { get; set; } // Фіксовані витрати
        public double VariableCostsPerUnit { get; set; } // Змінні витрати на одиницю
        public double PricePerUnit { get; set; } // Ціна за одиницю
        public int UnitsSold { get; set; } // Продані одиниці

        // Розрахунки KPI
        public double CalculateProfit() => Revenue - Expenses;
        public double CalculateProfitMargin() => Revenue > 0 ? (CalculateProfit() / Revenue) * 100 : 0;
        public double CalculateROI() => Investment > 0 ? (CalculateProfit() / Investment) * 100 : 0;
        public double CalculateBreakEven() => (PricePerUnit - VariableCostsPerUnit) > 0 ? FixedCosts / (PricePerUnit - VariableCostsPerUnit) : 0;
    }
}
