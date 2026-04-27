using CarService.Areas.Identity.Data;
using CarService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
