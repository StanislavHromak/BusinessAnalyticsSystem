using BusinessAnalyticsSystem.Controllers;
using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace BusinessAnalyticsSystem.Tests.ControllerTests
{
    public class AnalyticsControllerTests
    {
        // Допоміжний метод для створення нової фейкової БД для кожного тесту
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // Використовуємо InMemory та унікальне ім'я, щоб тести не конфліктували
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureCreated(); // Створюємо БД
            return context;
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var testData = new FinancialData
            {
                Id = 1,
                FixedCosts = 1000,
                PricePerUnit = 10,
                VariableCostPerUnit = 5,
                UnitsSold = 250 // (1000 / (10-5)) = 200 (точка беззбитковості)
            };

            testData.CalculateKPI(); 

            context.FinancialDatas.Add(testData);
            context.SaveChanges(); 

            var controller = new AnalyticsController(context);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Перевіряємо, що це View
            var model = Assert.IsAssignableFrom<FinancialData>(viewResult.Model); // Перевіряємо, що модель є FinancialData
            Assert.Equal(1, model.Id); // Перевіряємо, що це правильний запис
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext(); // Порожня БД
            var controller = new AnalyticsController(context);

            // Act
            var result = await controller.Details(99); // ID, якого не існує

            // Assert
            Assert.IsType<NotFoundResult>(result); // Перевіряємо, що повернуло "Not Found" [cite: 59]
        }

        [Fact]
        public async Task AddData_PostValidModel_SavesToDatabaseAndRedirects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new AnalyticsController(context);
            var viewModel = new AddDataViewModel
            {
                FixedCosts = 100,
                PricePerUnit = 10,
                VariableCostPerUnit = 5,
                UnitsSold = 30
            };

            // Act
            var result = await controller.AddData(viewModel);

            // Assert
            // 1. Перевіряємо, що нас перенаправило на "List" [cite: 57]
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);

            // 2. Перевіряємо, що дані реально збереглися в БД
            var savedData = await context.FinancialDatas.FirstOrDefaultAsync();
            Assert.NotNull(savedData);
            Assert.Equal(100, savedData.FixedCosts);

            // 3. Перевіряємо, що CalculateKPI був викликаний
            Assert.Equal(50, savedData.Profit); // (10*30) - (100 + 5*30) = 300 - 250 = 50. 
            // Давайте перерахуємо:
            // Revenue = 10 * 30 = 300
            // GrossCosts = 100 + (5 * 30) = 250
            // TotalCosts = 250 + 0 = 250
            // Profit = 300 - 250 = 50
            Assert.Equal(50, savedData.Profit);
        }

        [Fact]
        public async Task AddData_PostInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new AnalyticsController(context);
            var viewModel = new AddDataViewModel(); // Модель з помилками

            // "Штучно" додаємо помилку валідації
            controller.ModelState.AddModelError("FixedCosts", "Value cannot be negative.");

            // Act
            var result = await controller.AddData(viewModel);

            // Assert
            // 1. Перевіряємо, що нас повернуло на ту саму View [cite: 57] (але в іншому if-блоці)
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model); // Перевіряємо, що модель повернулася

            // 2. Перевіряємо, що в БД нічого не збереглося
            var count = await context.FinancialDatas.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
