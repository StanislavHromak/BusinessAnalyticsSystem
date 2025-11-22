using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessAnalyticsSystem.Models
{
    public class FinancialData
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        // Navigation property to related Sales (for aggregation)
        [NotMapped]
        public ICollection<Sale>? RelatedSales { get; set; }

        // ===== User input =====
        [Display(Name = "Fixed Costs")]
        [Range(0, double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        public double FixedCosts { get; set; }

        [Display(Name = "Variable Cost per Unit")]
        [Range(0, double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        public double VariableCostPerUnit { get; set; }

        [Display(Name = "Price per Unit")]
        [Range(0, double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        public double PricePerUnit { get; set; }

        [Display(Name = "Units Sold")]
        [Range(0, int.MaxValue, ErrorMessage = "Value cannot be negative.")]
        public int UnitsSold { get; set; }

        [Display(Name = "Investments")]
        [Range(0, double.MaxValue, ErrorMessage = "Value cannot be negative.")]
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

        // ===== Analysis =====
        [Display(Name = "Analysis & Recommendation")]
        public string AnalysisRecommendation { get; set; }

        // Flag to indicate if this FinancialData was generated from Sales
        [Display(Name = "Generated from Sales")]
        public bool IsGeneratedFromSales { get; set; } = false;

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
            GenerateAnalysis();
        }

        private void GenerateAnalysis()
        {
            if (Profit <= 0)
            {
                if (BreakEven > 0 && UnitsSold < BreakEven)
                {
                    AnalysisRecommendation = "Warning: Loss. Sales volume is below the break-even point. It is necessary to increase sales or reduce variable costs.";
                }
                else
                {
                    AnalysisRecommendation = "Warning: Loss. Review the cost structure (fixed and variable) and pricing policy.";
                }
            }
            else if (ROI < 5 && ROI > 0) // Assume 5% is a low threshold
            {
                AnalysisRecommendation = "Profitable, but low return on investment (ROI). Evaluate investment efficiency or seek ways to increase margin.";
            }
            else if (ROS < 10 && ROS > 0) // Assume 10% is low return on sales
            {
                AnalysisRecommendation = "Profitable, but low return on sales (ROS). Analyze margin (MarginPerUnit) and total costs.";
            }
            else
            {
                AnalysisRecommendation = "Healthy state: Business is profitable, sales are above the break-even point, and profitability indicators (ROI, ROS) are positive.";
            }
        }
    }
}


