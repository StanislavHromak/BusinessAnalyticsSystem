using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    public class FinancialData
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        // ===== User input =====
        [Display(Name = "Fixed Costs")]
        public double FixedCosts { get; set; }

        [Display(Name = "Variable Cost per Unit")]
        public double VariableCostPerUnit { get; set; }

        [Display(Name = "Price per Unit")]
        public double PricePerUnit { get; set; }

        [Display(Name = "Units Sold")]
        public int UnitsSold { get; set; }

        [Display(Name = "Investments")]
        public double Investment { get; set; }

        // ===== Calculated fields =====
        [Display(Name = "Gross Costs")]
        public double GrossCosts { get; set; } 

        [Display(Name = "Total Costs")]
        public double TotalCosts { get; set; } 

        [Display(Name = "Revenue")]
        public double Revenue { get; set; } 

        [Display(Name = "Profit")]
        public double Profit { get; set; } 

        [Display(Name = "Margin per Unit")]
        public double MarginPerUnit { get; set; }

        [Display(Name = "ROI (%)")]
        public double ROI { get; set; } 

        [Display(Name = "ROS (%)")]
        public double ROS { get; set; } 

        [Display(Name = "Break-Even Point (units)")]
        public double BreakEven { get; set; } 

        public void CalculateKPI()
        {
            Revenue = PricePerUnit * UnitsSold;
            GrossCosts = FixedCosts + VariableCostPerUnit * UnitsSold;
            TotalCosts = GrossCosts + Investment;
            Profit = Revenue - TotalCosts;
            MarginPerUnit = PricePerUnit - VariableCostPerUnit;
            ROI = Investment > 0 ? (Profit / Investment) * 100 : 0;
            ROS = Revenue > 0 ? (Profit / Revenue) * 100 : 0;
            BreakEven = (PricePerUnit - VariableCostPerUnit) > 0
                ? FixedCosts / (PricePerUnit - VariableCostPerUnit)
                : 0;
        }
    }
}

