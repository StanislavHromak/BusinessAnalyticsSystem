using BusinessAnalyticsSystem.Controllers;
using BusinessAnalyticsSystem.Data; 
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessAnalyticsSystem.Tests.ControllerTests
{
    // IDisposable потрібен для коректного очищення БД
    public class AccountControllerTests : IDisposable
    {
        private readonly AppDbContext _context; // Наша фейкова In-Memory БД
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;

        // Конструктор тесту налаштовує всі сервіси
        public AccountControllerTests()
        {
            // 1. Створюємо фейкову In-Memory DB
            _context = GetInMemoryDbContext();

            // 2. Створюємо моки
            _mockUserManager = GetMockUserManager();
            _mockSignInManager = GetMockSignInManager(_mockUserManager);

            // 3. (ВАЖЛИВО) "Згодовуємо" асинхронний DbSet з нашої БД
            // нашому моку UserManager. ТЕПЕР .Users.FirstOrDefaultAsync() ЗАПРАЦЮЄ.
            _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        }

        // Допоміжний метод для створення нової фейкової БД
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        // ----- (Решта Mock-методів залишається майже без змін) -----

        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Налаштування для CreateAsync (за замовчуванням успішне)
            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Налаштування для AddToRoleAsync
            mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mockUserManager;
        }

        private Mock<SignInManager<User>> GetMockSignInManager(Mock<UserManager<User>> mockUserManager)
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var loggerMock = new Mock<ILogger<SignInManager<User>>>();

            var mockSignInManager = new Mock<SignInManager<User>>(
                mockUserManager.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null, null, null, null);

            mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            mockSignInManager.Setup(m => m.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null))
                .Returns(Task.CompletedTask);

            return mockSignInManager;
        }

        // ----- ТЕСТИ -----

        [Fact]
        public async Task Register_Post_WithValidModel_CreatesUserAndRedirects()
        {
            // Arrange
            var controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
            var model = new RegisterViewModel
            {
                UserName = "testUser",
                FullName = "Test User",
                Email = "test@test.com",
                PhoneNumber = "+380991234567",
                Password = "Password1!"
            };

            // Act
            var result = await controller.Register(model);

            // Assert
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<User>(), model.Password), Times.Once);
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "Investor"), Times.Once);
            _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<User>(), false, null), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Register_Post_WhenUserExists_ReturnsViewWithError()
        {
            // Arrange
            // (1) Створюємо юзера в нашій In-Memory DB
            var existingUser = new User { UserName = "testUser", FullName = "Test Existing User", Email = "test@test.com", PhoneNumber = "+380991234567" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
            var model = new RegisterViewModel { UserName = "testUser" }; // Такий самий UserName

            // Act
            var result = await controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("User with this username, email, or phone already exists.", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            _mockSignInManager.Setup(m => m.PasswordSignInAsync("user", "wrongpass", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);

            // Act
            var result = await controller.Login("user", "wrongpass");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Invalid credentials.", controller.ViewBag.Error);
        }

        // Метод очищення для IDisposable
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
