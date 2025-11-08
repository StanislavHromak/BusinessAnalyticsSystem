using System.ComponentModel.DataAnnotations;

namespace BusinessAnalyticsSystem.Models
{
    public class AddDataViewModel
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

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
    }
}
