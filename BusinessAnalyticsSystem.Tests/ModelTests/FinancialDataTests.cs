using BusinessAnalyticsSystem.Models;
using Xunit;

namespace BusinessAnalyticsSystem.Tests.ModelTests
{
    public class FinancialDataTests
    {
        [Fact]
        public void CalculateKPI_CalculatesMetricsAndAnalysis_HealthyCase()
        {
            // Arrange (Вхідні дані)
            var model = new FinancialData
            {
                PricePerUnit = 200,
                VariableCostPerUnit = 50,
                FixedCosts = 1000,
                UnitsSold = 100,
                Investment = 5000
            };

            // Act (Дія)
            model.CalculateKPI();

            // Assert (Перевірка)
            Assert.Equal(20000, model.Revenue);
            Assert.Equal(6000, model.GrossCosts);
            Assert.Equal(11000, model.TotalCosts);
            Assert.Equal(9000, model.Profit);
            Assert.Equal(150, model.MarginPerUnit);
            Assert.Equal(180, model.ROI);
            Assert.Equal(45, model.ROS);
            Assert.Equal(6.67, model.BreakEven, 2);

            // ВИПРАВЛЕНО: Очікуємо англійський текст
            Assert.Equal("Healthy state: Business is profitable, sales are above the break-even point, and profitability indicators (ROI, ROS) are positive.", model.AnalysisRecommendation);
        }

        [Fact]
        public void CalculateKPI_CalculatesMetricsAndAnalysis_LossCase()
        {
            // Arrange
            var model = new FinancialData
            {
                PricePerUnit = 100,
                VariableCostPerUnit = 50,
                FixedCosts = 10000,
                UnitsSold = 50, // Менше ніж точка беззбитковості (200)
                Investment = 0
            };

            // Act
            model.CalculateKPI();

            // Assert
            Assert.Equal(5000, model.Revenue);
            Assert.Equal(12500, model.GrossCosts);
            Assert.Equal(-7500, model.Profit);
            Assert.Equal(200, model.BreakEven);

            // ВИПРАВЛЕНО: Очікуємо англійський текст
            Assert.Equal("Warning: Loss. Sales volume is below the break-even point. It is necessary to increase sales or reduce variable costs.", model.AnalysisRecommendation);
        }

        [Fact]
        public void CalculateKPI_HandlesDivideByZero_Safely()
        {
            // Arrange
            var model = new FinancialData
            {
                PricePerUnit = 10,
                VariableCostPerUnit = 10,
                UnitsSold = 0,
                Investment = 0
            };

            // Act
            model.CalculateKPI();

            // Assert
            Assert.Equal(0, model.ROI);
            Assert.Equal(0, model.ROS);
            Assert.Equal(0, model.BreakEven);
        }
    }
}
