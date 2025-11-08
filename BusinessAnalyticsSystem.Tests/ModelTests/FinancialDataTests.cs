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
            Assert.Equal(20000, model.Revenue); // 200 * 100
            Assert.Equal(6000, model.GrossCosts); // 1000 + (50 * 100)
            Assert.Equal(11000, model.TotalCosts); // 6000 + 5000
            Assert.Equal(9000, model.Profit); // 20000 - 11000
            Assert.Equal(150, model.MarginPerUnit); // 200 - 50
            Assert.Equal(180, model.ROI); // (9000 / 5000) * 100
            Assert.Equal(45, model.ROS); // (9000 / 20000) * 100
            Assert.Equal(6.67, model.BreakEven, 2); // 1000 / 150
            Assert.Equal("Здоровий стан: Бізнес прибутковий, продажі вищі за точку беззбитковості, а показники рентабельності (ROI, ROS) позитивні.", model.AnalysisRecommendation);
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
                UnitsSold = 50, // Менше ніж точка беззбитковості (10000 / 50 = 200)
                Investment = 0
            };

            // Act
            model.CalculateKPI();

            // Assert
            Assert.Equal(5000, model.Revenue); // 100 * 50
            Assert.Equal(12500, model.GrossCosts); // 10000 + (50 * 50)
            Assert.Equal(-7500, model.Profit); // 5000 - 12500
            Assert.Equal(200, model.BreakEven);
            Assert.Equal("Увага: Збиток. Обсяг продажів нижчий за точку беззбитковості. Необхідно збільшити продажі або знизити змінні витрати.", model.AnalysisRecommendation);
        }

        [Fact]
        public void CalculateKPI_HandlesDivideByZero_Safely()
        {
            // Arrange (Ситуації, що призводять до ділення на 0)
            var model = new FinancialData
            {
                PricePerUnit = 10,
                VariableCostPerUnit = 10, // -> BreakEven = 0
                UnitsSold = 0, // -> Revenue = 0 -> ROS = 0
                Investment = 0 // -> ROI = 0
            };

            // Act
            model.CalculateKPI();

            // Assert (Перевіряємо, що програма не впала і повернула 0)
            Assert.Equal(0, model.ROI);
            Assert.Equal(0, model.ROS);
            Assert.Equal(0, model.BreakEven);
        }
    }
}
