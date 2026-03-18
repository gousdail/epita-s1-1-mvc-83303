using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(RoleManager<IdentityRole> roleManager) : Controller
    {
        public async Task<IActionResult> Roles()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }
            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                await roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Roles));
        }
    }
}
