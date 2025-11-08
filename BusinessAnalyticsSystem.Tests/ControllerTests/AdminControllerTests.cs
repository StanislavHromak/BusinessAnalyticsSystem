using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BusinessAnalyticsSystem.Tests.ControllerTests
{
    public class AdminControllerTests
    {
        // Допоміжний метод для створення "фейкового" UserManager
        private Mock<UserManager<User>> GetMockUserManager()
        {
            // UserManager має складний конструктор, тому ми створюємо
            // "заглушку" для його залежності (IUserStore)
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            return userManagerMock;
        }

        // Допоміжний метод для "фейкового" RoleManager
        private Mock<RoleManager<IdentityRole<int>>> GetMockRoleManager()
        {
            var roleStoreMock = new Mock<IRoleStore<IdentityRole<int>>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole<int>>>(
                roleStoreMock.Object, null, null, null, null);
            return roleManagerMock;
        }


        [Fact]
        public async Task DeleteUser_WithValidId_CallsDeleteAsyncAndRedirects()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();
            var mockRoleManager = GetMockRoleManager();
            var user = new User { Id = 1, UserName = "TestUser" };

            // Налаштовуємо Mock: "Коли FindByIdAsync(1) буде викликано, поверни 'user'"
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);

            // Налаштовуємо Mock: "Коли DeleteAsync буде викликано, поверни 'Success'"
            mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var controller = new AdminController(mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.DeleteUser(1);

            // Assert
            // 1. Перевіряємо, що повернуло Redirect [cite: 51]
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UserManagement", redirectResult.ActionName);

            // 2. (Найважливіше) Перевіряємо, що метод DeleteAsync був викликаний 1 раз
            mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_DoesNotCallDeleteAndRedirects()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();
            var mockRoleManager = GetMockRoleManager();

            // Налаштовуємо Mock: "Коли FindByIdAsync(99) буде викликано, поверни null"
            mockUserManager.Setup(m => m.FindByIdAsync("99")).ReturnsAsync((User)null);

            var controller = new AdminController(mockUserManager.Object, mockRoleManager.Object);

            // Act
            var result = await controller.DeleteUser(99);

            // Assert
            // 1. Перевіряємо, що все одно повернуло Redirect [cite: 51]
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UserManagement", redirectResult.ActionName);

            // 2. (Найважливіше) Перевіряємо, що DeleteAsync НЕ БУВ викликаний [cite: 50]
            mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
