using CarService.Areas.Identity.Data;
using CarService.Models;
using CarService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarService.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdministrationController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> _userRoles = new Dictionary<string, string>();
            List<ApplicationUser> _users = await _context.Users.ToListAsync();
            for (int i = 0; i < _users.Count(); i++)
            {
                var role = await _userManager.GetRolesAsync(_users[i]);
                if(role.Count > 0)
                    _userRoles.Add(_users[i].Id, role[0].ToString());
            }

            var model = new AdministrationViewModel
            {
                users = _users,
                roles = await _roleManager.Roles.ToListAsync(),
                userRoles = _userRoles
            };
           
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new ApplicationUserViewModel
            {
                Roles = _roleManager.Roles.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SaveUser(ApplicationUserViewModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                Email = model.user.Email,
                UserName = model.user.Email,
                FirstName = model.user.FirstName,
                LastName = model.user.LastName,
                Age = model.user.Age,
                DocumentId = model.user.DocumentId,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.RoleName);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string userid)
        {
            if (!userid.IsNullOrEmpty())
            {
                ApplicationUser AppUser = await _userManager.FindByIdAsync(userid);
                if(AppUser != null)
                {
                    var viewModel = new ApplicationUserViewModel
                    {
                        Roles = _roleManager.Roles.ToList(),
                        user = AppUser
                    };
                    return View(viewModel);
                }
                return NotFound();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(ApplicationUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.user.Id);
                    user.Email = model.user.Email;
                    user.UserName = model.user.Email;
                    user.FirstName = model.user.FirstName;
                    user.LastName = model.user.LastName;
                    user.Age = model.user.Age;
                    user.DocumentId = model.user.DocumentId;

                    var res = await _userManager.UpdateAsync(user);
                    if (res.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Count > 0)
                        {
                            await _userManager.RemoveFromRoleAsync(user, roles[0]);
                            await _userManager.AddToRoleAsync(user, model.RoleName);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpGet, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return new NotFoundResult();
            }

            // Удаление пользователя
            var user = await _userManager.FindByIdAsync(id);
            var logins = await _userManager.GetLoginsAsync(user);
            var rolesForUser = await _userManager.GetRolesAsync(user);

            if (rolesForUser.Contains("User"))
            {
                // Открытие транзакции для комплексного удаления
                using (var transaction = _context.Database.BeginTransaction())
                {
                    // Удалить логин пользователя
                    foreach (var login in logins.ToList())
                    {
                        await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    }

                    // Удалить пользователя из ролей
                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }

                    // Удаление пользователя
                    await _userManager.DeleteAsync(user);

                    // Фиксация транзакции удаления
                    transaction.Commit();
                }
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}
