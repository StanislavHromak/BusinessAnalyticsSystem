using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public AdminController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> UserManagement()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModels = new List<UserViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            viewModels.Add(new UserViewModel { User = user, Role = roles.FirstOrDefault() ?? "No Role" });
        }
        return View(viewModels);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(int id, string newRole)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);

        return RedirectToAction(nameof(UserManagement));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(UserManagement));
    }
}

