using BusinessAnalyticsSystem.Data;
using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessAnalyticsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterView());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email || u.PhoneNumber == model.PhoneNumber);

                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "User with this username, email, or phone already exists.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Investor");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Dashboard));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileView
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileView model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var passwordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);  
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                ViewBag.Message = "Profile updated successfully!";
                return View(model);
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}


