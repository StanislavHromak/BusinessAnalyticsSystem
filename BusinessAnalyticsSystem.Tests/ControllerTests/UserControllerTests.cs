using BusinessAnalyticsSystem.Controllers;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BusinessAnalyticsSystem.Tests.ControllerTests
{
    public class UserControllerTests
    {
        // Допоміжний метод для створення фейкового UserManager
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Налаштування для GetUserAsync (щоб він повертав користувача)
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User { Id = 1, FullName = "Old Name" });

            // Налаштування для UpdateAsync (за замовчуванням успішне)
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Налаштування для ChangePasswordAsync (за замовчуванням успішне)
            mockUserManager.Setup(m => m.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mockUserManager;
        }

        [Fact]
        public async Task Profile_Post_ValidModel_NoPassword_UpdatesProfile()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();
            var controller = new UserController(mockUserManager.Object);

            // Імітуємо, що користувач автентифікований
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var model = new ProfileViewModel
            {
                FullName = "New Name",
                // NewPassword = null або ""
            };

            // Act
            var result = await controller.Profile(model);

            // Assert
            // 1. Перевіряємо, що повернуло View з повідомленням про успіх
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Profile updated successfully!", controller.ViewBag.Message);

            // 2. Перевіряємо, що UpdateAsync був викликаний
            mockUserManager.Verify(m => m.UpdateAsync(It.Is<User>(u => u.FullName == "New Name")), Times.Once);

            // 3. Перевіряємо, що ChangePasswordAsync НЕ був викликаний
            mockUserManager.Verify(m => m.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Profile_Post_ValidModel_WithPassword_UpdatesProfileAndPassword()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();
            var controller = new UserController(mockUserManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var model = new ProfileViewModel
            {
                FullName = "New Name",
                OldPassword = "Old",
                NewPassword = "NewPassword1!",
                ConfirmNewPassword = "NewPassword1!"
            };

            // Act
            var result = await controller.Profile(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Profile updated successfully!", controller.ViewBag.Message);

            // 2. Перевіряємо, що UpdateAsync був викликаний
            mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);

            // 3. Перевіряємо, що ChangePasswordAsync ТАКОЖ був викликаний
            mockUserManager.Verify(m => m.ChangePasswordAsync(It.IsAny<User>(), "Old", "NewPassword1!"), Times.Once);
        }

        [Fact]
        public async Task Profile_Post_PasswordChange_Fails_ReturnsViewWithError()
        {
            // Arrange
            var mockUserManager = GetMockUserManager();

            // Налаштовуємо ChangePasswordAsync на провал
            mockUserManager.Setup(m => m.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password error" }));

            var controller = new UserController(mockUserManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var model = new ProfileViewModel { NewPassword = "NewPassword1!" };

            // Act
            var result = await controller.Profile(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            // 1. Перевіряємо, що у ModelState є помилка
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Password error", controller.ModelState[string.Empty].Errors[0].ErrorMessage);

            // 2. Перевіряємо, що ViewBag.Message НЕ встановлено
            Assert.Null(controller.ViewBag.Message);
        }
    }
}